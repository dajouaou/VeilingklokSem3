const API_BASE = "https://localhost:56418";

export function iconUrl(path) {
    return `${API_BASE}/icons/${path}`;
}
