// src/api/VMApi.js

const API_BASE = import.meta.env.VITE_API_BASE;


if (!API_BASE) {
    console.error(
        "VITE_API_BASE is niet gezet. " +
        "Maak in de frontend-map een .env.local met bijvoorbeeld:\n" +
        "VITE_API_BASE=https://localhost:7140"
    );
}


async function safeFetch(path, options = {}) {
    const url = `${API_BASE}${path}`;

    const res = await fetch(url, {
     
        ...options,
    });

    const text = await res.text();

 
    if (!res.ok) {
        console.error("Serverfout:", res.status, text);
        throw new Error(`HTTP ${res.status}`);
    }

    // Proberen JSON te parsen
    if (!text) {
        return null; // lege body
    }

    try {
        return JSON.parse(text);
    } catch {
        console.error("Geen geldige JSON van server:");
        console.error(text);
        throw new Error("Server gaf geen JSON terug");
    }
}



// Dashboard ophalen
export function getDashboard(id) {
    return safeFetch(`/api/veilingen/${id}/vm/dashboard`);
}

// Start veiling
export function startVeiling(id) {
    return safeFetch(`/api/veilingen/${id}/vm/start`, {
        method: "POST",
    });
}

// Volgend product
export function nextProduct(id) {
    return safeFetch(`/api/veilingen/${id}/vm/next`, {
        method: "POST",
    });
}

// Huidig product sluiten
export function closeCurrent(id) {
    return safeFetch(`/api/veilingen/${id}/vm/close-current`, {
        method: "POST",
    });
}
