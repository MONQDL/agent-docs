# BufferedFileReaderPlugin

Пример потокового плагина. Плагин читает текстовый файл построчно и передаёт каждую строку агенту
через `PluginTaskContext.WriteOutput`. Буферизацией, формированием чанков и отправкой управляет агент
на основании секций `buffer` и `artifacts` непрерывного задания.

Команда плагина: `bufferedFileReaderPlugin`.

Пример задания:

```yaml
name: Buffered File Reader Task
continuous-jobs:
- name: buffered-file-reader
  buffer:
    type: filesystem
    input-name: buffered-file-reader.input
    chunk-size: 32768
  init-step:
    plugin: bufferedFileReaderPlugin
    with:
      filePath: C:\logs\application.log
    outputs:
      record: $._outputs
  artifacts:
  - type: logs
    send-to:
      monq:
        stream-key: "00000000-0000-0000-0000-000000000000"
    data: $.outputs.record
```

`buffer.type` поддерживает значения `memory` и `filesystem`. `input-name` должен быть уникальным
для одновременно работающих входов, а `chunk-size` задаётся в байтах.
