namespace Stratus
{
	/// <summary>
	/// Action is the base class from which all other actions derive from.
	/// </summary>
	public abstract class StratusAction
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/    
		/// <summary>
		/// A private identifier for this action.
		/// </summary>
		public int id { get; private set; }
		/// <summary>
		/// How much time has elapsed since the action started running
		/// </summary>
		public float elapsed { get; protected set; }
		/// <summary>
		/// The total amount of time the action will run for
		/// </summary>
		public float duration { get; protected set; }
		/// <summary>
		/// Whether the action is currently active. If not active it may end up
		/// blocking others behind it (if its on a sequence).
		/// </summary>
		public bool isActive { get; private set; }
		/// <summary>
		/// Whether the action has finished running.
		/// </summary>
		public bool isFinished = false;
		/// <summary>
		/// Whether we are logging actions
		/// </summary>
		protected static bool logging = false;

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// How many actions have been created so far
		/// </summary>
		private static int created = 0;

		/// <summary>
		/// How many actions have been destroyed so far
		/// </summary>
		private static int destroyed = 0;

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		public abstract float Update(float dt);

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusAction()
		{
			this.id = created++;
		}

		~StratusAction()
		{
			destroyed++;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Resumes running the actionn. It will no longer block any actions beyond it in a sequence.
		/// </summary>
		public void Resume()
		{
			this.isActive = true;
		}

		/// <summary>
		/// Pauses the update of this action. This will block a sequence 
		/// if there's other actions behind it.
		/// </summary>
		public void Pause()
		{
			this.isActive = false;
		}

		/// <summary>
		/// Cancels execution of this action. It will be cleaned up at the next opportunity.
		/// </summary>
		public void Cancel()
		{
			this.isActive = false;
			this.isFinished = true;
		}


	}


}

