namespace Funzilla
{
	internal class LevelSelector : OptimizedScrollViewY
	{
		private void Start()
		{
			Init(LevelManager.Levels.Count);
		}

		private void OnEnable()
		{
			MoveTo(Profile.Level - 1);
		}
	}
}

