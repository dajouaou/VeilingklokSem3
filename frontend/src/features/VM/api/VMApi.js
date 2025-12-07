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

    if (!res.ok) {
        const message = text || `HTTP ${res.status}`;
        console.error("Serverfout:", res.status, message);
        throw new Error(message);
    }

    if (!text) return null;

    try {
        return JSON.parse(text);
    } catch {
        console.error("Geen geldige JSON van server:");
        console.error(text);
        throw new Error("Server gaf geen geldige JSON terug");
    }
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
