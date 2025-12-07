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

export async function fetchVeilDagenForAanvoerder(token) {
    const res = await fetch(`${API_BASE}/api/veiling-public/dagen`, {
        headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) throw new Error("Kon veildagen niet laden.");
    return res.json();
}

export async function createVeildag(token, datum) {
    const res = await fetch(`${API_BASE}/api/veiling-public/dagen`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ datum })
    });

    if (!res.ok) throw new Error("Kon veildag niet aanmaken.");
    return res.json();
}
