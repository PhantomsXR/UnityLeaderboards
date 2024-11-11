using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Leaderboards.Internal.Scheduler
{
    /// <summary>
    /// Helper class for referencing TaskScheduler in HttpClient when making
    // an asynchronous web request. Also provides a reference to the main thread id.
    /// </summary>
    internal static class ThreadHelper
    {
        public static SynchronizationContext SynchronizationContext => _unitySynchronizationContext;
        public static System.Threading.Tasks.TaskScheduler TaskScheduler => _taskScheduler;
        public static int MainThreadId => _mainThreadId;

        private static SynchronizationContext _unitySynchronizationContext;
        private static System.Threading.Tasks.TaskScheduler _taskScheduler;
        private static int _mainThreadId;

        /// <summary>
        /// Init runs at start sets the main thread ID to ensure that methods which can only be
        /// called from the main thread have a reference. It is also triggered
        /// by switching between Runtime and Editor to update the thread id.
        /// </summary>
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            _unitySynchronizationContext = SynchronizationContext.Current;
            _taskScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
