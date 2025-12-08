const API_BASE = "https://localhost:56418";

export async function fetchAanmeldingen({ token, veildatum }) {
    const params = new URLSearchParams();
    if (veildatum) params.set("veildatum", veildatum);

    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen?${params}`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) throw new Error("Kon aanmeldingen niet laden.");
    return res.json();
}

export async function createAanmelding({ token, data }) {
    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(data),
    });

    if (!res.ok) throw new Error("Kon aanmelding niet opslaan.");
    return res.json();
}

export async function fetchAanvoerderStats({ token, veildatum }) {
    const params = new URLSearchParams();
    if (veildatum) params.set("veildatum", veildatum);

    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/statistieken?${params}`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) throw new Error("Kon statistieken niet laden.");
    return res.json();
}
export async function updateAanmelding({ token, id, data }) {
    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) throw new Error("Kon aanmelding niet wijzigen.");
    return res.json();
}

export async function deleteAanmelding({ token, id }) {
    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen/${id}`, {
        method: "DELETE",
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    if (!res.ok) throw new Error("Kon aanmelding niet verwijderen.");
}
