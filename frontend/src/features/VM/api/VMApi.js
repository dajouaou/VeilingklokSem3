// src/api/VMApi.js

const API_BASE = import.meta.env.VITE_API_BASE ?? "";

if (!API_BASE) {
    console.error(
        "VITE_API_BASE is niet gezet. Maak in frontend/.env.local bijvoorbeeld:\n" +
        "VITE_API_BASE=https://localhost:7140"
    );
}

async function safeFetch(path, options = {}) {
    const url = `${API_BASE}${path}`;
    const res = await fetch(url, options);
    const text = await res.text();
    let json = null;

    if (text) {
        try {
            json = JSON.parse(text);
        } catch {
            if (res.ok) {
                console.error("Server gaf geen geldige JSON terug:", text);
            }
        }
    }

    if (!res.ok) {
        const message =
            json?.message ||
            json?.title ||
            text ||
            `HTTP ${res.status}`;

        const error = new Error(message);
        error.status = res.status;
        error.body = json;
        throw error;
    }

    return json;
}

export function getDashboard(veilingId) {
    return safeFetch(`/api/veilingen/${veilingId}/vm/dashboard`);
}

export function startVeiling(veilingId) {
    return safeFetch(`/api/veilingen/${veilingId}/vm/start`, {
        method: "POST",
    });
}

export function nextProduct(veilingId) {
    return safeFetch(`/api/veilingen/${veilingId}/vm/next`, {
        method: "POST",
    });
}

export function closeCurrent(veilingId) {
    return safeFetch(`/api/veilingen/${veilingId}/vm/close-current`, {
        method: "POST",
    });
}
