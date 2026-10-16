using UnityEngine;

/// <summary>
/// DollBook_Page UI buttons drive page changes through NextPage and PreviousPage.
/// Page visuals can be assigned later without changing the button setup.
/// </summary>
public class DollBookPageController : MonoBehaviour
{
    [Header("Page Setup")]
    [Tooltip("Number of pages in the book. Set this when page content is ready.")]
    [SerializeField, Min(1)] private int pageCount = 1;

    [Tooltip("Zero-based starting page index.")]
    [SerializeField, Min(0)] private int startingPage;

    [Header("Optional Page Visuals")]
    [Tooltip("Optional page GameObjects, one for each page. Materials can be added later.")]
    [SerializeField] private GameObject[] pageObjects;

    private int currentPage;

    public int CurrentPage => currentPage;
    public int PageCount => pageCount;

    private void Awake()
    {
        currentPage = Mathf.Clamp(startingPage, 0, Mathf.Max(0, pageCount - 1));
        ApplyPageVisibility();
    }

    /// <summary>Assign this method to the next-page button's OnClick event.</summary>
    public void NextPage()
    {
        if (currentPage >= pageCount - 1)
        {
            return;
        }

        currentPage++;
        ApplyPageVisibility();
    }

    /// <summary>Assign this method to the previous-page button's OnClick event.</summary>
    public void PreviousPage()
    {
        if (currentPage <= 0)
        {
            return;
        }

        currentPage--;
        ApplyPageVisibility();
    }

    /// <summary>Sets the page count when page content is created or updated.</summary>
    public void SetPageCount(int count)
    {
        pageCount = Mathf.Max(1, count);
        currentPage = Mathf.Clamp(currentPage, 0, pageCount - 1);
        ApplyPageVisibility();
    }

    private void ApplyPageVisibility()
    {
        if (pageObjects == null)
        {
            return;
        }

        for (int i = 0; i < pageObjects.Length; i++)
        {
            if (pageObjects[i] != null)
            {
                pageObjects[i].SetActive(i == currentPage);
            }
        }
    }
}