# SystemMetricsPlugin

Базовый пример плагина для формирования метрик в текстовом формате Prometheus. Плагин возвращает
общий объём дисков и процент занятого пространства.

Команда плагина: `systemMetrics`.

Пример задания:

```yaml
name: System Metrics Gathering Task
jobs:
- name: System Metrics Gathering Job
  steps:
  - plugin: systemMetrics
    with:
      customFields:
        source: system-metrics
        agentName: $.agentName
    outputs:
      result: $._outputs.result
  artifacts:
  - type: metrics
    send-to: monq
    data: $.outputs.result
```
