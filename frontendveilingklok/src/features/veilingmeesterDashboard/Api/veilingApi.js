// core/features/veilingmeesterDashboard/api/veilingApi.js
import { httpClient } from "../../../core/api/httpClient.js";



// Alle veilingen ophalen (optioneel filter op veildatum)
export async function fetchVeilingen({ token, veildatum }) {
    const response = await httpClient.get("/veilingen", {
        params: veildatum ? { veildatum } : {},
        headers: { Authorization: `Bearer ${token}` },
    });
    return response.data; // array van veilingen
}

// Nieuwe veiling plannen
export async function createVeiling({ token, data }) {
    const response = await httpClient.post("/veilingen", data, {
        headers: { Authorization: `Bearer ${token}` },
    });
    return response.data; // aangemaakte veiling
}

// Veiling wijzigen
export async function updateVeiling({ token, id, data }) {
    const response = await httpClient.put(`/veilingen/${id}`, data, {
        headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
}

// Veiling verwijderen
export async function deleteVeiling({ token, id }) {
    await httpClient.delete(`/veilingen/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
    });
}

// Veiling starten (status -> Gestart, bijv.)
export async function startVeiling({ token, id }) {
    const response = await httpClient.post(`/veilingen/${id}/start`, null, {
        headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
}

// Producten van aanvoerders ophalen voor bepaalde veildatum
export async function fetchAanvoerderProducten({ token, veildatum }) {
    const response = await httpClient.get("/aanvoerder/producten", {
        params: { veildatum },
        headers: { Authorization: `Bearer ${token}` },
    });
    return response.data; // array van producten van aanvoerders
}
