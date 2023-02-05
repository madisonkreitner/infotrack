using System;
using System.Collections.Concurrent;
using static Googler.Services.IServiceHealthState;

namespace Googler.Services
{
    /// <summary>
    /// Maintains state of the last time a background service performed a checkin
    /// </summary>
    /// <remarks>
    /// NOTE ON LOCKING: Locks are not used in this code because assigning a DateTime object is equivalent to assigning an int64 value [sizeof(DateTime) == 8]
    ///            ECMA "1.12.6.6 Atomic reads and writes" states that any structure aligned with the native word size of the system is naturally atomic and doesn't require additional syncing
    ///            It is assumed that all apps using this code will be running on at least a 64-bit platform and therefore no sync locking is provided as it will just add blocking overhead with no actual safety benefit.
    /// </remarks>
    public class ServiceHealthState : IServiceHealthState
    {
		#region Fields
		private readonly ConcurrentDictionary<TopicProcessorId, DateTime> _checkinTime = new ConcurrentDictionary<TopicProcessorId, DateTime>();
		#endregion

		#region Methods
		/// <summary>
		/// Retrieve the last successful health check datetime.
		/// </summary>
		/// <returns>Datetime</returns>
		public DateTime GetHealthState(TopicProcessorId topicProcessorId)
		{
			return _checkinTime.TryGetValue(topicProcessorId, out DateTime tm) ? tm : DateTime.UtcNow;
		}

		/// <summary>
		/// Update the health check datatime to Utc Now.
		/// </summary>
		public void SetHealthState(TopicProcessorId topicProcessorId)
		{
			_checkinTime[topicProcessorId] = DateTime.UtcNow;
		}
		#endregion
	}
}
