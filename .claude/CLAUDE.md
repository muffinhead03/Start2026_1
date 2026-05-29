# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Do Not Open This Box** — a Unity 6000.3.11f1 (URP) 1st-person horror escape-room game for Windows/Mac. Players escape 6 sequential rooms; each scene has its own puzzle gimmick. A local on-device LLM (Ollama, `gemma3:4b`) provides player-tailored hints over an in-game radio UI, limited to 2 uses per scene.

Source language: C#. UI text/comments are largely in Korean.

## Architecture

- Interaction System (raycast-driven): [Interaction](./guidance/Interaction.md)
- Hint System: [Hint](./guidance/Hint.md)

## For More Details

- See [Readme](README.md) for project overview and repository architecture.
- See [TeamGroundRule](Team_Ground_Rule.md) for commit or push.
