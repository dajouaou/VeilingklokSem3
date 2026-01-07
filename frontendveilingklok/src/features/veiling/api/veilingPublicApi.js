const API_BASE = "https://localhost:56418";

async function apiGet(url) {
    const res = await fetch(`${API_BASE}${url}`);
    let data = null;
    try { data = await res.json(); } catch { }
    if (!res.ok) return null;
    return data;
}

export function getPublicActieveVeiling() {
    return apiGet("/api/veiling-public/actief");
}

export function getPublicVolgendeVeiling() {
    return apiGet("/api/veiling-public/volgende");
}
