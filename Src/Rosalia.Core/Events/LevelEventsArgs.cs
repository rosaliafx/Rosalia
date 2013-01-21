namespace Rosalia.Core.Events
{
    using System;

    public class LevelEventsArgs : EventArgs
    {
        public LevelEventsArgs(int currentLevel)
        {
            CurrentLevel = currentLevel;
        }

        public int CurrentLevel { get; private set; }
    }
}