using UnityEngine;

/// <summary>
/// DollBook_Page UI buttons drive page changes through NextPage and PreviousPage.
/// The selected page material is written to MeshRenderer material elements 0-2.
/// </summary>
public class DollBookPageController : MonoBehaviour
{
    [Header("Page Setup")]
    [Tooltip("Materials for pages 0-2. The selected material is applied to renderer elements 0-2.")]
    [SerializeField] private Material[] pageMaterials = new Material[3];

    [Tooltip("Zero-based starting page index.")]
    [SerializeField, Min(0)] private int startingPage;

    [Header("Book Renderer")]
    [Tooltip("Mesh Renderer whose material elements 0-2 contain the book pages.")]
    [SerializeField] private MeshRenderer bookRenderer;

    [Tooltip("Optional Animator. Set pageTurnTrigger to a trigger parameter configured for page turns in its controller.")]
    [SerializeField] private Animator bookAnimator;

    [SerializeField] private string pageTurnTrigger = "Push";
    private int pageTurnTriggerHash;

    private void OnValidate()
    {
        pageTurnTriggerHash = Animator.StringToHash(pageTurnTrigger ?? string.Empty);
    }

    [Header("Optional Page Objects")]
    [Tooltip("Optional page GameObjects, one for each page.")]
    [SerializeField] private GameObject[] pageObjects;

    private int currentPage;

    public int CurrentPage => currentPage;
    public int PageCount => Mathf.Clamp(pageMaterials == null ? 0 : pageMaterials.Length, 1, 3);

    private void Awake()
    {
        currentPage = Mathf.Clamp(startingPage, 0, Mathf.Max(0, PageCount - 1));
        ApplyPage();
    }

    /// <summary>Assign this method to the next-page button's OnClick event.</summary>
    public void NextPage()
    {
        if (currentPage >= PageCount - 1)
        {
            PlayPageTurnTrigger();
            return;
        }

        currentPage++;
        ApplyPage();
    }

    /// <summary>Assign this method to the previous-page button's OnClick event.</summary>
    public void PreviousPage()
    {
        if (currentPage <= 0)
        {
            PlayPageTurnTrigger();
            return;
        }

        currentPage--;
        ApplyPage();
    }

    /// <summary>Sets the page count when page content is created or updated.</summary>
    public void SetPageCount(int count)
    {
        int materialCount = Mathf.Clamp(count, 1, 3);
        if (pageMaterials == null || pageMaterials.Length != materialCount)
        {
            System.Array.Resize(ref pageMaterials, materialCount);
        }

        currentPage = Mathf.Clamp(currentPage, 0, PageCount - 1);
        ApplyPage();
    }

    private void ApplyPage()
    {
        if (bookRenderer != null && pageMaterials != null && pageMaterials.Length > currentPage)
        {
            Material selectedMaterial = pageMaterials[currentPage];
            if (selectedMaterial != null)
            {
                Material[] materials = bookRenderer.sharedMaterials;
                if (materials.Length >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        materials[i] = selectedMaterial;
                    }

                    bookRenderer.sharedMaterials = materials;
                }
            }
        }

        if (pageObjects != null)
        {
            for (int i = 0; i < pageObjects.Length; i++)
            {
                if (pageObjects[i] != null)
                {
                    pageObjects[i].SetActive(i == currentPage);
                }
            }
        }

        PlayPageTurnTrigger();
    }

    private void PlayPageTurnTrigger()
    {
        if (bookAnimator != null && !string.IsNullOrEmpty(pageTurnTrigger))
        {
            if (pageTurnTriggerHash == 0)
            {
                pageTurnTriggerHash = Animator.StringToHash(pageTurnTrigger);
            }

            bookAnimator.SetTrigger(pageTurnTriggerHash);
        }
    }
}