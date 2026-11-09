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
})(jQuery);
