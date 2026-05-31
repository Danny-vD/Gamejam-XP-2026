using UnityEngine;
using VDFramework.ObserverPattern;

namespace XPGJ2026.MenuScripts.Pages
{
	public class OrderedPageManager : PageManager
	{
		/// <summary>
		/// CurrentPage | Max Pages
		/// </summary>
		public readonly PrioritisedAction<int, int> OnPageBack = new PrioritisedAction<int, int>();

		/// <summary>
		/// CurrentPage | Max Pages
		/// </summary>
		public readonly PrioritisedAction<int, int> OnPageForward = new PrioritisedAction<int, int>();

		/// <summary>
		/// CurrentPage | Max Pages
		/// </summary>
		public readonly PrioritisedAction<int, int> OnPageChanged = new PrioritisedAction<int, int>();

		public readonly PrioritisedAction<bool> OnCanGoBackwardsChanged = new PrioritisedAction<bool>();
		public readonly PrioritisedAction<bool> OnCanGoForwardsChanged = new PrioritisedAction<bool>();

		[Header("Settings")]
		[SerializeField, Tooltip("If true, you can use move past the first and last page to loop around to the last and first page respectively")]
		private bool loop = false;

		[field: SerializeField, Tooltip("If true, the number of the first page will be '0' instead of '1'")]
		public bool ZeroIndexed { get; private set; }

		public int CurrentPageNumber { get; private set; }

		public int MaxPageNumber { get; private set; }
		public int LowestPageNumber { get; private set; }

		private bool canGoBackwards = false;
		
		public bool CanGoBackwards
		{
			get => canGoBackwards;
			private set
			{
				if (value != canGoBackwards)
				{
					canGoBackwards = value;
					OnCanGoBackwardsChanged.Invoke(canGoBackwards);
				}
			}
		}

		private bool canGoForwards = false;
		
		public bool CanGoForwards
		{
			get => canGoForwards;
			private set
			{
				if (value != canGoForwards)
				{
					canGoForwards = value;
					OnCanGoForwardsChanged.Invoke(canGoForwards);
				}
			}
		}

		private void Awake()
		{
			if (ZeroIndexed)
			{
				LowestPageNumber = 0;
				MaxPageNumber    = pages.Length - 1;
			}
			else
			{
				LowestPageNumber = 1;
				MaxPageNumber    = pages.Length;
			}
		}

		protected override void Start()
		{
			base.Start();
			
			if (!loop)
			{
				if (CurrentPageNumber != LowestPageNumber)
				{
					CanGoBackwards = true;
				}
				else
				{
					CanGoBackwards = false;
					OnCanGoBackwardsChanged.Invoke(false); // It doesn't automatically invoke this because it hasn't actually changed (defaults to false)
				}

				if (CurrentPageNumber != MaxPageNumber) // Could be both at the same time if there's only 1 page
				{
					CanGoForwards = true;
				}
				else
				{
					CanGoForwards = false;
					OnCanGoForwardsChanged.Invoke(false); // It doesn't automatically invoke this because it hasn't actually changed (defaults to false)
				}
			}
			else
			{
				CanGoBackwards = true;
				CanGoForwards  = true;
			}
		}

		public void GoPageBack()
		{
			if (!CanGoBackwards)
			{
				return;
			}

			if (CurrentPageNumber == LowestPageNumber) // Loops around, no need to check if 'loop' since the boolean above wouldn't allow it
			{
				CurrentPageNumber = MaxPageNumber;
			}
			else
			{
				--CurrentPageNumber;

				if (!loop && CurrentPageNumber == LowestPageNumber) // If we're now at the beginning
				{
					CanGoBackwards = false;
				}

				CanGoForwards = true; // We went backward, so naturally we can go forwards
			}

			GoToPage(pages[GetIndexFromPageNumber(CurrentPageNumber)]);

			OnPageBack.Invoke(CurrentPageNumber, MaxPageNumber);
			OnPageChanged.Invoke(CurrentPageNumber, MaxPageNumber);
		}

		public void GoPageForward()
		{
			if (!CanGoForwards)
			{
				return;
			}

			if (CurrentPageNumber == MaxPageNumber) // Loops around, no need to check if 'loop' since the boolean above wouldn't allow it
			{
				CurrentPageNumber = LowestPageNumber;
			}
			else
			{
				++CurrentPageNumber;

				if (!loop && CurrentPageNumber == MaxPageNumber) // If we're now at the end
				{
					CanGoForwards = false;
				}

				CanGoBackwards = true; // We went forward, so naturally we can go backwards
			}

			GoToPage(pages[GetIndexFromPageNumber(CurrentPageNumber)]);

			OnPageForward.Invoke(CurrentPageNumber, MaxPageNumber);
			OnPageChanged.Invoke(CurrentPageNumber, MaxPageNumber);
		}

		private int GetIndexFromPageNumber(int pageNumber)
		{
			if (ZeroIndexed)
			{
				return pageNumber;
			}

			return pageNumber - 1;
		}

		private int GetPageNumberFromIndex(int index)
		{
			if (ZeroIndexed)
			{
				return index;
			}

			return index + 1;
		}

		private bool TryGetPageNumberFromPage(Page page, out int pageNumber)
		{
			for (pageNumber = 0; pageNumber < pages.Length; pageNumber++)
			{
				Page currentPage = pages[pageNumber];

				if (currentPage == page)
				{
					if (!ZeroIndexed)
					{
						++pageNumber;
					}

					return true;
				}
			}

			pageNumber = -1;
			return false;
		}

		public override void GoToStartPage()
		{
			base.GoToStartPage();
			
			if (TryGetPageNumberFromPage(startPage, out int pageNumber))
			{
				CurrentPageNumber = pageNumber;
				OnPageChanged.Invoke(CurrentPageNumber, MaxPageNumber);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			OnPageBack.ClearCallbacks();
			OnPageForward.ClearCallbacks();

			OnPageChanged.ClearCallbacks();
		}
	}
}