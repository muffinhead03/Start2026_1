#!/usr/bin/env python3
"""
download_model.py

힌트 시스템에서 사용하는 로컬 LLM 모델(.gguf)을 다운로드하고
SHA256 체크섬으로 무결성을 검증하는 스크립트.

사용법:
    python download_model.py                # 기본 모델(Gemma 3 4B) 다운로드
    python download_model.py --model gemma3-4b --output ./models
    python download_model.py --list         # 등록된 모델 목록 보기
    python download_model.py --verify-only ./models/gemma-3-4b-it-Q4_K_M.gguf

목적:
    - "가격 우위성" 주장을 뒷받침 — 클라우드 API 없이 로컬 모델만으로 동작함을
      누구나 재현 가능하게 하기 위함
    - 잘못된/손상된 다운로드로 인한 힌트 시스템 오작동 방지
"""

import argparse
import hashlib
import os
import sys
import urllib.request

# ── 등록된 모델 목록 ─────────────────────────────────────────────
# 새 모델을 추가하려면 여기에 항목을 하나 더 넣으면 됨.
# sha256은 Hugging Face 파일 페이지의 "SHA256:" 값을 그대로 복사.
MODELS = {
    "gemma3-4b": {
        "name": "Gemma 3 4B (Q4_K_M)",
        "url": "https://huggingface.co/lmstudio-community/gemma-3-4b-it-GGUF/resolve/main/gemma-3-4b-it-Q4_K_M.gguf",
        "filename": "gemma-3-4b-it-Q4_K_M.gguf",
        "sha256": "be49949e48422e4547b00af14179a193d3777eea7fbbd7d6e1b0861304628a01",
    },
    # 필요하면 아래처럼 추가 (SHA256은 각 모델의 HuggingFace 파일 페이지에서 확인):
    # "qwen3.5-4b": {
    #     "name": "Qwen 3.5 4B (Q4_K_M)",
    #     "url": "https://huggingface.co/unsloth/Qwen3.5-4B-GGUF/resolve/main/Qwen3.5-4B-Q4_K_M.gguf",
    #     "filename": "Qwen3.5-4B-Q4_K_M.gguf",
    #     "sha256": "",  # TODO: 채워넣기
    # },
}

DEFAULT_MODEL_KEY = "gemma3-4b"
CHUNK_SIZE = 1024 * 1024  # 1MB


def human_size(num_bytes: float) -> str:
    for unit in ["B", "KB", "MB", "GB"]:
        if num_bytes < 1024:
            return f"{num_bytes:.1f}{unit}"
        num_bytes /= 1024
    return f"{num_bytes:.1f}TB"


def compute_sha256(path: str) -> str:
    """파일 전체를 스트리밍으로 읽어 SHA256 해시를 계산."""
    sha256 = hashlib.sha256()
    with open(path, "rb") as f:
        while chunk := f.read(CHUNK_SIZE):
            sha256.update(chunk)
    return sha256.hexdigest()


def download_with_progress(url: str, dest_path: str) -> None:
    """URL에서 파일을 받아 dest_path에 저장하며 진행률을 표시."""
    req = urllib.request.Request(url, headers={"User-Agent": "Mozilla/5.0"})
    with urllib.request.urlopen(req) as response:
        total = int(response.headers.get("Content-Length", 0))
        downloaded = 0

        with open(dest_path, "wb") as out_file:
            while chunk := response.read(CHUNK_SIZE):
                out_file.write(chunk)
                downloaded += len(chunk)
                if total > 0:
                    pct = downloaded / total * 100
                    bar_len = 30
                    filled = int(bar_len * downloaded / total)
                    bar = "#" * filled + "-" * (bar_len - filled)
                    print(
                        f"\r[{bar}] {pct:5.1f}%  "
                        f"({human_size(downloaded)} / {human_size(total)})",
                        end="",
                        flush=True,
                    )
                else:
                    print(f"\r{human_size(downloaded)} 다운로드됨...", end="", flush=True)
    print()  # 줄바꿈


def verify_checksum(path: str, expected_sha256: str) -> bool:
    if not expected_sha256:
        print("⚠️  등록된 SHA256이 없어 검증을 건너뜁니다.")
        return True

    print("SHA256 검증 중...")
    actual = compute_sha256(path)

    if actual.lower() == expected_sha256.lower():
        print(f"✅ 체크섬 일치: {actual}")
        return True
    else:
        print("❌ 체크섬 불일치!")
        print(f"   기대값: {expected_sha256}")
        print(f"   실제값: {actual}")
        return False


def list_models() -> None:
    print("등록된 모델:")
    for key, info in MODELS.items():
        marker = " (기본값)" if key == DEFAULT_MODEL_KEY else ""
        print(f"  - {key}{marker}: {info['name']}")


def main() -> int:
    parser = argparse.ArgumentParser(description="힌트 시스템용 로컬 LLM 모델 다운로드 + 검증")
    parser.add_argument(
        "--model",
        default=DEFAULT_MODEL_KEY,
        help=f"다운로드할 모델 키 (기본값: {DEFAULT_MODEL_KEY})",
    )
    parser.add_argument(
        "--output",
        default="./models",
        help="모델을 저장할 디렉터리 (기본값: ./models)",
    )
    parser.add_argument(
        "--list",
        action="store_true",
        help="등록된 모델 목록만 출력하고 종료",
    )
    parser.add_argument(
        "--verify-only",
        metavar="PATH",
        help="다운로드 없이 지정한 파일의 SHA256만 검증",
    )
    args = parser.parse_args()

    if args.list:
        list_models()
        return 0

    if args.verify_only:
        # 파일명으로 등록된 모델을 역추적해서 기대 해시를 찾음
        filename = os.path.basename(args.verify_only)
        expected = ""
        for info in MODELS.values():
            if info["filename"] == filename:
                expected = info["sha256"]
                break
        if not os.path.exists(args.verify_only):
            print(f"❌ 파일이 없습니다: {args.verify_only}")
            return 1
        ok = verify_checksum(args.verify_only, expected)
        return 0 if ok else 1

    if args.model not in MODELS:
        print(f"❌ 알 수 없는 모델: {args.model}")
        list_models()
        return 1

    info = MODELS[args.model]
    os.makedirs(args.output, exist_ok=True)
    dest_path = os.path.join(args.output, info["filename"])

    if os.path.exists(dest_path):
        print(f"이미 파일이 존재합니다: {dest_path}")
        print("기존 파일을 검증합니다...")
        if verify_checksum(dest_path, info["sha256"]):
            print("기존 파일이 유효합니다. 다운로드를 건너뜁니다.")
            return 0
        else:
            print("기존 파일이 손상된 것으로 보입니다. 다시 다운로드합니다.")

    print(f"다운로드 시작: {info['name']}")
    print(f"URL: {info['url']}")
    print(f"저장 위치: {dest_path}")

    try:
        download_with_progress(info["url"], dest_path)
    except Exception as e:
        print(f"❌ 다운로드 실패: {e}")
        return 1

    if not verify_checksum(dest_path, info["sha256"]):
        print("❌ 검증 실패 — 다운로드된 파일이 손상되었을 수 있습니다. 파일을 삭제합니다.")
        os.remove(dest_path)
        return 1

    print(f"✅ 완료: {dest_path}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
