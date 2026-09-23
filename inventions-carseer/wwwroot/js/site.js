document.addEventListener('DOMContentLoaded', () => {
    const makeSelect = document.getElementById('makeSelect');
    const typeSelect = document.getElementById('vehicleTypeSelect');
    if (!makeSelect || !typeSelect) {
        return;
    }

    // Tom Select hides the original <select>, so let client-side validation still check it.
    const validationIgnore = ':hidden:not(#makeSelect)';
    if (window.jQuery?.validator) {
        jQuery.validator.setDefaults({ ignore: validationIgnore });
        const validator = jQuery(makeSelect.form).data('validator');
        if (validator) {
            validator.settings.ignore = validationIgnore;
        }
    }

    // Searchable dropdowns (desktop and mobile). maxOptions keeps rendering fast; typing searches the full list.
    const makeTs = new TomSelect(makeSelect, {
        placeholder: 'Search a make...',
        maxOptions: 300,
        allowEmptyOption: false
    });

    const typeTs = new TomSelect(typeSelect, {
        placeholder: 'Any type',
        allowEmptyOption: true
    });

    const resetTypes = () => {
        typeTs.clear(true);
        typeTs.clearOptions();
        typeTs.addOption({ value: '', text: 'Any type' });
        typeTs.setValue('', true);
    };

    makeTs.on('change', () => {
        if (window.jQuery && jQuery(makeSelect.form).data('validator')) {
            jQuery(makeSelect).valid();
        }
    });

    // Loader shown while the search results page loads.
    const form = makeSelect.form;
    const searchButton = document.getElementById('searchButton');
    const searchLoader = document.getElementById('searchLoader');
    const setLoading = loading => {
        if (searchLoader) {
            searchLoader.hidden = !loading;
        }
        if (searchButton) {
            searchButton.querySelector('.spinner-border').hidden = !loading;
            searchButton.querySelector('.btn-label').textContent = loading ? 'Searching...' : 'Search';
            searchButton.disabled = loading;
        }
    };

    form.addEventListener('submit', () => {
        if (window.jQuery && jQuery(form).data('validator') && !jQuery(form).valid()) {
            return;
        }
        // Disable after the submit has started so the request still goes through.
        setTimeout(() => setLoading(true), 0);
    });

    // Reset when the page is restored from the back/forward cache.
    window.addEventListener('pageshow', () => setLoading(false));

    makeSelect.addEventListener('change', async () => {
        resetTypes();
        if (makeSelect.value) {
            typeTs.enable();
        } else {
            typeTs.disable();
            return;
        }

        try {
            const url = `${makeSelect.dataset.typesUrl}?makeId=${encodeURIComponent(makeSelect.value)}`;
            const response = await fetch(url);
            if (!response.ok) {
                return;
            }

            const types = await response.json();
            typeTs.addOptions(types.map(type => ({ value: type.name, text: type.name })));
        } catch {
            resetTypes();
        }
    });
});
