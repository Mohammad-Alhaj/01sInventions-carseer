document.addEventListener('DOMContentLoaded', () => {
    const makeSelect = document.getElementById('makeSelect');
    const typeSelect = document.getElementById('vehicleTypeSelect');
    if (!makeSelect || !typeSelect) {
        return;
    }

    const resetTypes = () => {
        typeSelect.replaceChildren(new Option('Any type', ''));
    };

    makeSelect.addEventListener('change', async () => {
        resetTypes();
        typeSelect.disabled = !makeSelect.value;
        if (!makeSelect.value) {
            return;
        }

        try {
            const url = `${makeSelect.dataset.typesUrl}?makeId=${encodeURIComponent(makeSelect.value)}`;
            const response = await fetch(url);
            if (!response.ok) {
                return;
            }

            const types = await response.json();
            types.forEach(type => typeSelect.add(new Option(type.name, type.name)));
        } catch {
            resetTypes();
        }
    });
});
