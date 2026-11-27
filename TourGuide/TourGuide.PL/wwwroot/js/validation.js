// Client adapters for the custom attributes in TourGuide.PL/Validation (AGENTS.md -> Validation).
(function ($) {
    if (!$.validator || !$.validator.unobtrusive) return;

    // [FutureDate(hours)] on a date input ("yyyy-mm-dd"): the chosen day must end at least `hours` from now.
    // Rough filter in the browser's clock; the server decides with the guide's time zone.
    $.validator.addMethod('futuredate', function (value, element, hours) {
        if (this.optional(element)) return true;
        const parts = value.split('-').map(Number);
        if (parts.length !== 3 || parts.some(isNaN)) return false;
        const endOfDay = new Date(parts[0], parts[1] - 1, parts[2], 23, 59, 59);
        return endOfDay.getTime() >= Date.now() + Number(hours) * 3600 * 1000;
    });
    $.validator.unobtrusive.adapters.addSingleVal('futuredate', 'hours');
    // [MaxFileSize]: the chosen file must not be larger than `bytes`.
    $.validator.addMethod('maxfilesize', function (value, element, bytes) {
        if (!element.files || element.files.length === 0) return true;
        return element.files[0].size <= Number(bytes);
    });
    $.validator.unobtrusive.adapters.addSingleVal('maxfilesize', 'bytes');

    // [AllowedFileTypes]: the file name must end with one of the comma-separated `extensions`.
    $.validator.addMethod('allowedfiletypes', function (value, element, extensions) {
        if (!element.files || element.files.length === 0) return true;
        const name = element.files[0].name.toLowerCase();
        return String(extensions).split(',').some(function (ext) {
            return name.endsWith(ext.trim().toLowerCase());
        });
    });
    $.validator.unobtrusive.adapters.addSingleVal('allowedfiletypes', 'extensions');
})(jQuery);
