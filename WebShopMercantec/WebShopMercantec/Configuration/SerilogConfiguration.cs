using Serilog;
using Serilog.Events;

namespace WebShopMercantec.Configuration;

/// <summary>
/// Конфигурация Serilog для структурированного логирования
/// 
/// ЧТО ТАКОЕ SERILOG?
/// Библиотека для логирования, которая:
/// - Пишет логи в файлы, консоль, БД
/// - Структурированные логи (JSON)
/// - Удобная фильтрация по уровням
/// - Контекстная информация
/// 
/// УРОВНИ ЛОГОВ:
/// 1. Verbose - всё подряд (обычно не используется)
/// 2. Debug - отладочная информация
/// 3. Information - обычная работа (запросы, события)
/// 4. Warning - предупреждения (не критично)
/// 5. Error - ошибки (исключения)
/// 6. Fatal - критические ошибки (падение приложения)
/// 
/// ГДЕ ХРАНЯТСЯ ЛОГИ:
/// - logs/webshop-YYYYMMDD.txt (файлы с ротацией по дням)
/// - Console (во время разработки)
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Настроить Serilog
    /// Вызывается в Program.cs перед созданием builder
    /// </summary>
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            // Минимальный уровень логирования
            .MinimumLevel.Information()
            
            // Переопределяем уровни для системных логов
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            
            // Обогащаем логи контекстом
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            
            // Пишем в консоль (удобно для разработки)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            
            // Пишем в файлы с ротацией по дням
            .WriteTo.File(
                path: "logs/webshop-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                retainedFileCountLimit: 30, // Храним последние 30 файлов
                fileSizeLimitBytes: 10 * 1024 * 1024, // Макс 10 MB на файл
                rollOnFileSizeLimit: true)
            
            // Создаем logger
            .CreateLogger();
    }
}

