const API_BASE = import.meta.env.VITE_API_BASE;

// veilige fetch
async function safeFetch(url, options = {}) {
    const res = await fetch(url, options);
    const text = await res.text();

    try {
        return JSON.parse(text);
    } catch {
        console.error("Geen geldige JSON van server:");
        console.error(text);
        throw new Error("Server gaf geen JSON terug");
    }
}

// Dashboard ophalen
export async function getDashboard(id) {
    return safeFetch(`${API_BASE}/api/veilingen/${id}/vm/dashboard`);
}

// Start veiling
export async function startVeiling(id) {
    return safeFetch(`${API_BASE}/api/veilingen/${id}/vm/start`, {
        method: "POST",
    });
}

// Volgend product
export async function nextProduct(id) {
    return safeFetch(`${API_BASE}/api/veilingen/${id}/vm/next`, {
        method: "POST",
    });
}

// Sluit product
export async function closeCurrent(id) {
    return safeFetch(`${API_BASE}/api/veilingen/${id}/vm/close-current`, {
        method: "POST",
    });
}
