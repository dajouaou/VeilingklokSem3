import { API_BASE_URL } from "../../../config/apiBaseUrl";

async function apiGet(path) {
    const res = await fetch(`${API_BASE_URL}${path}`);

    let data = null;
    try {
        data = await res.json();
    } catch { }

    if (!res.ok) {
        throw new Error(`GET ${path} mislukt`);
    }

    return data;
}

export function getPublicActieveVeiling() {
    return apiGet("/api/veiling-public/actief");
}

export function getPublicVolgendeVeiling() {
    return apiGet("/api/veiling-public/volgende");
}
