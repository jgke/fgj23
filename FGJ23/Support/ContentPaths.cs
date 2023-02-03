


namespace FGJ23
{
    class Content
    {
        public static class Files
        {
#if WINDOWS
			public const string Player = @"Content\Files\player.png";
#else
            public const string Player = @"Content/Files/player.png";
#endif
#if WINDOWS
			public const string HealthPickup = @"Content\Files\HealthPickup.png";
#else
            public const string HealthPickup = @"Content/Files/HealthPickup.png";
#endif
#if WINDOWS
			public const string Bone = @"Content\Files\bone.png";
#else
            public const string Bone = @"Content/Files/bone.png";
#endif
#if WINDOWS
			public const string FloatingBlob = @"Content\Files\FloatingBlob.png";
#else
            public const string FloatingBlob = @"Content/Files/FloatingBlob.png";
#endif
#if WINDOWS
			public const string Bullet = @"Content\Files\Bullet.png";
#else
            public const string Bullet = @"Content/Files/Bullet.png";
#endif
        }

#if WINDOWS
		public const string Icon = @"Content\Icon.ico";
#else
        public const string Icon = @"Content/Icon.ico";
#endif

    }
}



