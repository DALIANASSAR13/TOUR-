// Confirm dialog for forms that cannot be undone (Withdraw, Reject).
// A form with data-confirm-title is stopped on submit and sent only after "Yes" in the dialog.
// Without this script, or in a browser without <dialog>, the form posts directly.
(function () {
    const dialog = document.getElementById('confirm-dialog');
    if (!dialog || typeof dialog.showModal !== 'function') return;

    const title = dialog.querySelector('#confirm-dialog-title');
    const text = dialog.querySelector('#confirm-dialog-text');
    const ok = dialog.querySelector('[data-dialog-ok]');
    const cancel = dialog.querySelector('[data-dialog-cancel]');
    let pendingForm = null;

    document.addEventListener('submit', function (e) {
        const form = e.target;
        if (!(form instanceof HTMLFormElement) || !form.dataset.confirmTitle) return;

        e.preventDefault();
        pendingForm = form;
        title.textContent = form.dataset.confirmTitle;
        text.textContent = form.dataset.confirmText || '';
        ok.textContent = form.dataset.confirmOk || 'Confirm';
        cancel.textContent = form.dataset.confirmCancel || 'Go back';
        ok.disabled = false;
        dialog.showModal(); // focus lands on the safe button (cancel)
    });

    ok.addEventListener('click', function () {
        if (!pendingForm) return;
        ok.disabled = true;   // no double post
        pendingForm.submit(); // does not fire "submit" again
    });

    cancel.addEventListener('click', function () { dialog.close(); });

    // A click on the dark area outside the box closes it. Esc closes it natively.
    dialog.addEventListener('click', function (e) {
        const box = dialog.getBoundingClientRect();
        const inside = e.clientX >= box.left && e.clientX <= box.right && e.clientY >= box.top && e.clientY <= box.bottom;
        if (!inside) dialog.close();
    });

    dialog.addEventListener('close', function () { pendingForm = null; });
})();
