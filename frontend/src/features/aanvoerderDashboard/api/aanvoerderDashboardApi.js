const API_BASE = import.meta.env.VITE_API_BASE;

async function readError(res) {
    const ct = res.headers.get("content-type") || "";
    if (ct.includes("application/json")) {
        try {
            const j = await res.json();
            return typeof j === "string" ? j : j?.message || j?.error || JSON.stringify(j);
        } catch {}
    }
    try {
        const t = await res.text();
        return t || `HTTP ${res.status}`;
    } catch {
        return `HTTP ${res.status}`;
    }
}

async function handle(res) {
    if (!res.ok) {
        const msg = await readError(res);
        if (res.status === 401) throw new Error(`Niet ingelogd of token ongeldig. (${msg})`);
        if (res.status === 403) throw new Error(`Geen toegang (rol). (${msg})`);
        throw new Error(msg || `HTTP ${res.status}`);
    }
    if (res.status === 204) return null;
    return res.json();
}

function qs(params) {
    const sp = new URLSearchParams();
    Object.entries(params || {}).forEach(([k, v]) => {
        if (v === undefined || v === null || v === "") return;
        sp.set(k, v);
    });
    const s = sp.toString();
    return s ? `?${s}` : "";
}

function getToken(token) {
    return token || localStorage.getItem("token") || "";
}

function authHeaders(token) {
    const t = getToken(token);
    return t ? { Authorization: `Bearer ${t}` } : {};
}

function buildAanmeldingFormData(dto) {
    const fd = new FormData();
    fd.append("Soort", dto.soort ?? "");
    fd.append("Hoeveelheid", String(dto.hoeveelheid ?? 0));
    fd.append("MinimumPrijs", String(dto.minimumPrijs ?? 0));
    fd.append("KlokLocatie", String(dto.klokLocatie ?? ""));
    fd.append("LeverDatum", dto.leverDatum ?? "");
    if (dto.potmaat) fd.append("Potmaat", dto.potmaat);
    if (dto.steellengte) fd.append("Steellengte", dto.steellengte);
    if (dto.beschrijving) fd.append("Beschrijving", dto.beschrijving);
    if (dto.foto instanceof File) fd.append("Foto", dto.foto);
    return fd;
}

export async function getAanmeldingen({ leverdatum } = {}, token) {
    const url = `${API_BASE}/api/aanvoerder/dashboard/aanmeldingen${qs({ leverdatum })}`;
    const res = await fetch(url, { headers: authHeaders(token) });
    return handle(res);
}

export async function getStatistieken({ leverdatum } = {}, token) {
    const url = `${API_BASE}/api/aanvoerder/dashboard/statistieken${qs({ leverdatum })}`;
    const res = await fetch(url, { headers: authHeaders(token) });
    return handle(res);
}

export async function getVeildagen(token) {
    const url = `${API_BASE}/api/aanvoerder/dashboard/veildagen`;
    const res = await fetch(url, { headers: authHeaders(token) });
    return handle(res);
}

export async function createAanmelding(dto, token) {
    const url = `${API_BASE}/api/aanvoerder/dashboard/aanmeldingen`;
    const res = await fetch(url, { method: "POST", body: buildAanmeldingFormData(dto), headers: authHeaders(token) });
    return handle(res);
}

export async function updateAanmelding(id, dto, token) {
    const url = `${API_BASE}/api/aanvoerder/dashboard/aanmeldingen/${id}`;
    const res = await fetch(url, { method: "PUT", body: buildAanmeldingFormData(dto), headers: authHeaders(token) });
    return handle(res);
}

export async function deleteAanmelding(id, token) {
    const url = `${API_BASE}/api/aanvoerder/dashboard/aanmeldingen/${id}`;
    const res = await fetch(url, { method: "DELETE", headers: authHeaders(token) });
    return handle(res);
}

