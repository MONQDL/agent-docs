# SystemInfoPlugin

Базовый пример плагина для формирования лога. Плагин возвращает имя компьютера и добавляет к
записи пользовательские поля из `customFields`.

Команда плагина: `systemInfoPlugin`.

Пример задания:

```yaml
name: System Info Gathering Task
jobs:
- name: System Info Gathering Job
  steps:
  - plugin: systemInfoPlugin
    with:
      customFields:
        source: system-info
        agentName: $.agentName
    outputs:
      result: $._outputs.result
  artifacts:
  - type: logs
    send-to: monq
    data: $.outputs.result
```

Пример также показывает регистрацию собственного сервиса в `PluginTaskBootstrap` и его внедрение
в `PluginTaskStrategy` через DI.
