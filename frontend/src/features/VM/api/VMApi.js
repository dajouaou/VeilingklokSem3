// src/api/VMApi.js

// 1. Basis URL van je backend (uit .env.local)
const API_BASE = import.meta.env.VITE_API_BASE;

// Kleine check: als je .env.local mist, zie je dit in de console
if (!API_BASE) {
    console.error(
        "VITE_API_BASE is niet gezet. Maak in frontend/.env.local bijvoorbeeld:\n" +
        "VITE_API_BASE=https://localhost:7140"
    );
}

// 2. Veilige fetch helper
async function safeFetch(path, options = {}) {
    // path = "/api/veilingen/1/vm/dashboard"
    const url = `${API_BASE}${path}`;

    const res = await fetch(url, options);
    const text = await res.text();

    // 2a. HTTP-fouten (400, 404, 500, …)
    if (!res.ok) {
        // hier pakken we de tekst van de server, bv "Geen volgende producten."
        const message = text || `HTTP ${res.status}`;
        console.error("Serverfout:", res.status, message);
        throw new Error(message); // → err.message in je hook
    }

    // 2b. Lege body (bijv. 204 No Content)
    if (!text) return null;

    // 2c. Proberen JSON te parsen
    try {
        return JSON.parse(text);
    } catch {
        console.error("Geen geldige JSON van server:");
        console.error(text);
        throw new Error("Server gaf geen JSON terug");
    }
}

// 3. API-functies voor veilingmeester

export function getDashboard(id) {
    return safeFetch(`/api/veilingen/${id}/vm/dashboard`);
}

export function startVeiling(id) {
    return safeFetch(`/api/veilingen/${id}/vm/start`, {
        method: "POST",
    });
}

export function nextProduct(id) {
    return safeFetch(`/api/veilingen/${id}/vm/next`, {
        method: "POST",
    });
}

export function closeCurrent(id) {
    return safeFetch(`/api/veilingen/${id}/vm/close-current`, {
        method: "POST",
    });
}
