const API_BASE = "https://localhost:56418";

export async function fetchAanmeldingen({ token, leverdatum }) {
    let url = `${API_BASE}/api/aanvoerder/dashboard/aanmeldingen`;

    if (leverdatum) {
        url += `?leverdatum=${leverdatum}`;
    }

    const res = await fetch(url, {
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
    formData.append("LeverDatum", data.leverdatum);
    formData.append("Beschrijving", data.beschrijving ?? "");


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



export async function fetchAanvoerderStats({ token, leverdatum }) {
    let url = `${API_BASE}/api/aanvoerder/dashboard/statistieken`;

    if (leverdatum) {
        url += `?leverdatum=${leverdatum}`;
    }

    const res = await fetch(url, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) throw new Error("Kon statistieken niet laden.");
    return res.json();
}


export async function updateAanmelding({ token, id, data }) {
    const formData = new FormData();

    formData.append("Soort", data.soort);
    formData.append("Potmaat", data.potmaat ?? "");
    formData.append("Steellengte", data.steellengte ?? "");
    formData.append("Hoeveelheid", data.hoeveelheid);
    formData.append("MinimumPrijs", data.minimumPrijs);
    formData.append("KlokLocatie", data.klokLocatie);
    formData.append("LeverDatum", data.leverdatum);
    formData.append("Beschrijving", data.beschrijving ?? "");


    if (data.fotoFile) {
        formData.append("Foto", data.fotoFile);
    }

    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen/${id}`, {
        method: "PUT",
        headers: { Authorization: `Bearer ${token}` },
        body: formData
    });

    if (!res.ok) throw new Error("Kon aanmelding niet wijzigen.");
    return res.json();
}



export async function deleteAanmelding({ token, id }) {
    const res = await fetch(`${API_BASE}/api/aanvoerder/dashboard/aanmeldingen/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
    });

    let data = null;
    try {
        data = await res.json();
    } catch {
        // empty
    }

    if (!res.ok) {
        const msg =
            typeof data === "string"
                ? data
                : data?.message || data?.title || "Kon aanmelding niet verwijderen.";

        throw new Error(msg);
    }
}
