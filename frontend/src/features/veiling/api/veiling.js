// src/api/veiling.js
import { apiGet, apiPost } from "./http";

export function getPublicVeiling(veilingId) {
    return apiGet(`/api/veilingen/${veilingId}/public`);
}

export function placeBidApi(veilingId, koperId) {
    const query = new URLSearchParams({ koperId: String(koperId) }).toString();
    return apiPost(`/api/veilingen/${veilingId}/bids?${query}`, {});
}
