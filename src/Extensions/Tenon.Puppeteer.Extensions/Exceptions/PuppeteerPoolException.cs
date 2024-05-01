namespace Tenon.Puppeteer.Extensions.Exceptions;

public class PuppeteerPoolException(string message, Exception ex = null) : InvalidOperationException(message, ex);