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
    const formData = new FormData();
    formData.append("Soort", data.soort);
    formData.append("Potmaat", data.potmaat ?? "");
    formData.append("Steellengte", data.steellengte ?? "");
    formData.append("Hoeveelheid", data.hoeveelheid);
    formData.append("MinimumPrijs", data.minimumPrijs);
    formData.append("KlokLocatie", data.klokLocatie);
    formData.append("Veildatum", data.veildatum);

    if (data.fotoFile) {
        formData.append("Foto", data.fotoFile);
    }

    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen`, {
        method: "POST",
        headers: {
            Authorization: `Bearer ${token}`,
        },
        body: formData
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
