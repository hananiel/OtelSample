# OtelSample
 Using Steeltoe extension methods with Otel might show the error message: "Please ensure OpenTelemetry is configured via Steeltoe extension methods".
 The error message is saying that you need to use the Steeltoe extension for Metrics: `AddOpenTelemetryMetricsForSteeltoe to get the desired functionality. 
 You need to do this instead of calling the Otel extension method to configure metrics. 
 Steeltoe internally uses OpentelemetryMetrics for its own exports for example /Metrics, /Prometheus and also Wavefront Exporter. 
 
 This sample shows how to use them together. 
