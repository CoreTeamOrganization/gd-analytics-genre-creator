#nullable enable
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Serialization;
#if METICA_ANALYTICS
using Metica;
#endif

namespace GameDistrict.MeticaAnalytics
{
    public class GDMeticaAnalytics : ScriptableObject
    {
        [Header("Metica Configuration")]
        [SerializeField] private string appId     = "";
        [SerializeField] private string apiKey    = "";
        
        [Tooltip("Leave empty to use SystemInfo.deviceUniqueIdentifier")]
        [SerializeField] private string userId    = "";

        [Header("Adjust Information")]
        [SerializeField] private string adId      = "";
        [SerializeField] private string appToken  = "";

        public string? AbTest          { get; set; } = null;
        public string? AbGroup         { get; set; } = null;
        public string? AbTestStartDate { get; set; } = null;
        
        /// <summary>
        /// Manually initialize Metica SDK. Call this when Auto Initialize is unchecked.
        /// Optionally pass a userId to override the inspector value and SystemInfo.deviceUniqueIdentifier.
        /// </summary>
        public void Initialize(string? overrideUserId = null)
        {
#if METICA_ANALYTICS
            RunInit(overrideUserId);
#endif
        }

        private void RunInit(string? overrideUserId)
        {
#if METICA_ANALYTICS
            var resolvedUserId = !string.IsNullOrEmpty(overrideUserId) ? overrideUserId
                : !string.IsNullOrEmpty(userId) ? userId
                : SystemInfo.deviceUniqueIdentifier;

            MeticaSdk.InitializeAnalytics(new MeticaInitConfig(apiKey, appId, resolvedUserId));
#endif
        }

        /// <summary>
        /// Update the Adjust ad ID and app token after async Adjust initialization.
        /// </summary>
        public void UpdateAdjustInfo(string adId, string appToken)
        {
            this.adId      = adId;
            this.appToken  = appToken;
        }

        public virtual Dictionary<string, object> CreateBaseEvent()
        {
            return WithAbTestFields(new Dictionary<string, object>
            {
                { "adid",      adId },
                { "appToken", appToken },
            });
        }

        private Dictionary<string, object> WithAbTestFields(Dictionary<string, object> payload)
        {
            if (!string.IsNullOrEmpty(AbTest))          payload["abTest"]          = AbTest;
            if (!string.IsNullOrEmpty(AbGroup))         payload["abGroup"]         = AbGroup;
            if (!string.IsNullOrEmpty(AbTestStartDate)) payload["abTestStartDate"] = AbTestStartDate;
            return payload;
        }

        protected void MergeCustomFields(Dictionary<string, object> payload, AnalyticsEventData data)
        {
            if (data.CustomFields == null) return;
            foreach (var kvp in data.CustomFields)
                payload[kvp.Key] = kvp.Value;
        }

        private Dictionary<string, object> WithBaseFields(Dictionary<string, object>? payload)
        {
            var result = new Dictionary<string, object>
            {
                { "adid",      adId },
                { "appToken", appToken },
            };
            if (payload != null)
                foreach (var kvp in payload)
                    result[kvp.Key] = kvp.Value;
            return WithAbTestFields(result);
        }

        #region metica-events
        public virtual void LogPurchaseEvent(string productId, string currency, double amount, string status, string? errorCode, string? referenceId, Dictionary<string, object>? customPayload)
        {
#if METICA_ANALYTICS
            var mergedPayload = WithBaseFields(customPayload);
            MeticaSdk.Analytics.LogPurchaseEvent(productId, currency, amount, status, errorCode, referenceId, mergedPayload);
            Debug.Log($"[MeticaAnalytics] LogPurchaseEvent: {productId}, {currency}, {amount}, {status}\nPayload: {JsonConvert.SerializeObject(mergedPayload)}");
#endif
        }

        public virtual void LogSessionStartEvent(Dictionary<string, object>? customPayload)
        {
#if METICA_ANALYTICS
            var mergedPayload = WithBaseFields(customPayload);
            MeticaSdk.Analytics.LogSessionStartEvent(mergedPayload);
            Debug.Log($"[MeticaAnalytics] LogSessionStartEvent\nPayload: {JsonConvert.SerializeObject(mergedPayload)}");
#endif
        }

        public virtual void LogInstallEvent(Dictionary<string, object>? customPayload)
        {
#if METICA_ANALYTICS
            var mergedPayload = WithBaseFields(customPayload);
            MeticaSdk.Analytics.LogInstallEvent(mergedPayload);
            Debug.Log($"[MeticaAnalytics] LogInstallEvent\nPayload: {JsonConvert.SerializeObject(mergedPayload)}");
#endif
        }

        public virtual void LogImpressionEvent(double value, string type, string mediator, string source, string? placement, Dictionary<string, object>? customPayload)
        {
#if METICA_ANALYTICS
            var mergedPayload = WithBaseFields(customPayload);
            MeticaSdk.Analytics.LogImpressionEvent(value, type, mediator, source, placement, mergedPayload);
            Debug.Log($"[MeticaAnalytics] LogImpressionEvent: {value}, {type}, {mediator}, {source}\nPayload: {JsonConvert.SerializeObject(mergedPayload)}");
#endif
        }

        public virtual void LogFullStateUpdateEvent(Dictionary<string, object> attributes)
        {
#if METICA_ANALYTICS
            var mergedAttributes = WithBaseFields(attributes);
            MeticaSdk.Analytics.LogFullStateUpdateEvent(mergedAttributes);
            Debug.Log($"[MeticaAnalytics] LogFullStateUpdateEvent: {mergedAttributes.Count} attributes\nAttributes: {JsonConvert.SerializeObject(mergedAttributes)}");
#endif
        }

        public virtual void LogPartialStateUpdateEvent(Dictionary<string, object> attributes)
        {
#if METICA_ANALYTICS
            var mergedAttributes = WithBaseFields(attributes);
            MeticaSdk.Analytics.LogPartialStateUpdateEvent(mergedAttributes);
            Debug.Log($"[MeticaAnalytics] LogPartialStateUpdateEvent: {mergedAttributes.Count} attributes\nAttributes: {JsonConvert.SerializeObject(mergedAttributes)}");
#endif
        }

        public virtual void LogCustomEvent(string eventName, Dictionary<string, object>? properties)
        {
#if METICA_ANALYTICS
            MeticaSdk.Analytics.LogCustomEvent(eventName, properties);
            var propertiesJson = properties != null ? JsonConvert.SerializeObject(new { properties }) : "null";
            Debug.Log($"[MeticaAnalytics] LogCustomEvent: {eventName}\nProperties: {propertiesJson}");
#endif
        }
        #endregion
    }
}