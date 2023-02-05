using System;

namespace Googler.Services
{
    /// <summary>
    /// Checkin process for determing the health of a background worker service
    /// </summary>
    public interface IServiceHealthState
    {
		/// <summary>
		/// The Id of the background processor that we want to collect health information on
		/// </summary>
		enum TopicProcessorId
		{
			/// <summary>
			/// Background service representing the processing of the main topic
			/// </summary>
			MainTopic,

			/// <summary>
			/// Background service representing the processing of the retry topic
			/// </summary>
			RetryTopic
		}

		/// <summary>
		/// Perform a Check in
		/// </summary>
		void SetHealthState(TopicProcessorId topicProcessorId);

		/// <summary>
		/// Return the last time a background worker service checked in
		/// </summary>
		/// <returns>Time of the last checkin</returns>
		DateTime GetHealthState(TopicProcessorId topicProcessorId);
	}
}