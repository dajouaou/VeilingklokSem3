// src/api/veilingmeester.js
import { apiGet, apiPost, apiPut } from "./api";

export const VeilingApi = {
    // READ
    details: (id) => apiGet(`/veilingen/${id}`),
    currentLot: (id) => apiGet(`/veilingen/${id}/current`),
    queue: (id) => apiGet(`/veilingen/${id}/queue`),
    queueAll: (id) => apiGet(`/veilingen/${id}/queue/all`),
    bids: (id) => apiGet(`/veilingen/${id}/bids`),
    audit: (id) => apiGet(`/veilingen/${id}/audit`),

    // COMMANDS
    start: (body) => apiPost(`/veilingen`, body),
    pause: (id) => apiPost(`/veilingen/${id}/pause`, {}),
    resume: (id) => apiPost(`/veilingen/${id}/resume`, {}),
    stop: (id) => apiPost(`/veilingen/${id}/stop`, {}),

    addQueueItem: (id, body) => apiPost(`/veilingen/${id}/queue`, body),
    reorderQueue: (id, body) => apiPut(`/veilingen/${id}/queue/reorder`, body),

    placeBid: (id, body) => apiPost(`/veilingen/${id}/bids`, body),
};
