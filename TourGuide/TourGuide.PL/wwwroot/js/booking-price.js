// FR-12: live price preview on the booking form. The server computes the price; this only asks and shows it.
(function ($) {
    const $form = $('#booking-form');
    if (!$form.length) return;

    const $box = $('#price-box');
    const $total = $box.find('[data-total]');
    const $breakdown = $box.find('[data-breakdown]');
    const $error = $box.find('[data-error]');
    const url = $form.data('preview-url');
    let timer = null;
    let pending = null;

    // Show only the inputs of the chosen type; disabled inputs are not sent.
    function applyType() {
        const type = $form.find('input[name="BookingType"]:checked').val();
        $form.find('.bk-group').each(function () {
            const active = $(this).data('type') === type;
            $(this).toggle(active).find('input').prop('disabled', !active);
        });
    }

    function showError(message) {
        $total.text('—');
        $breakdown.text('');
        $error.text(message || '');
    }

    function preview() {
        const $amount = $form.find('input[name="Hours"]:enabled, input[name="Days"]:enabled');
        if (!$amount.val() || ($.fn.valid && !$amount.valid())) {
            showError('');
            return;
        }

        if (pending) pending.abort();
        pending = $.post(url, $form.serialize())
            .done(function (q) {
                $total.text(q.totalText);
                $breakdown.text(q.breakdown);
                $error.text('');
            })
            .fail(function (xhr) {
                if (xhr.statusText === 'abort') return;
                const body = xhr.responseJSON || {};
                const first = body.errors && Object.values(body.errors)[0];
                showError((first && first[0]) || body.message || 'Could not get the price. Try again.');
            })
            .always(function () { pending = null; });
    }

    function schedule() {
        clearTimeout(timer);
        timer = setTimeout(preview, 250);
    }

    // Whole positive numbers only: block "-", "+", "e" and decimals; strip them from pasted text.
    $form.on('keydown', 'input[name="Hours"], input[name="Days"]', function (e) {
        if (['-', '+', 'e', 'E', '.', ','].includes(e.key)) e.preventDefault();
    });
    $form.on('input', 'input[name="Hours"], input[name="Days"]', function () {
        const digits = this.value.replace(/[^0-9]/g, '');
        if (digits !== this.value) this.value = digits;
    });

    $form.on('change', 'input[name="BookingType"]', function () { applyType(); schedule(); });
    $form.on('input change', 'input[name="Hours"], input[name="Days"]', schedule);

    applyType();
})(jQuery);
