import { useMemo, useState } from "react";
import { useAanvoerderDashboard } from "../hooks/useAanvoerderDashboard";

function money(n) {
    const v = Number(n ?? 0);
    return new Intl.NumberFormat("nl-NL", { style: "currency", currency: "EUR" }).format(v);
}

function dateNL(d) {
    if (!d) return "";
    const dt = new Date(d);
    if (Number.isNaN(dt.getTime())) return String(d);
    return dt.toLocaleDateString("nl-NL");
}

const KLOKLOCATIES = ["Aalsmeer", "Naaldwijk"];

export default function AanvoerderDashboardPage({ token, aanvoerderNaam = "Aanvoerder" }) {
    const {
        loading,
        saving,
        error,
        leverdatum,
        setLeverdatum,
        veildagen,
        zoek,
        setZoek,
        statusFilter,
        setStatusFilter,
        filteredAanmeldingen,
        totals,
        selected,
        setSelected,
        create,
        update,
        remove,
    } = useAanvoerderDashboard({ token });

    const [view, setView] = useState("overzicht");

    const [formOpen, setFormOpen] = useState(false);
    const [editId, setEditId] = useState(null);
    const [form, setForm] = useState({
        soort: "",
        potmaat: "",
        steellengte: "",
        hoeveelheid: 1,
        minimumPrijs: 0.01,
        klokLocatie: "Aalsmeer",
        leverDatum: "",
        beschrijving: "",
        foto: null,
    });

    function openCreate() {
        setEditId(null);
        setForm({
            soort: "",
            potmaat: "",
            steellengte: "",
            hoeveelheid: 1,
            minimumPrijs: 0.01,
            klokLocatie: "Aalsmeer",
            leverDatum: leverdatum || "",
            beschrijving: "",
            foto: null,
        });
        setFormOpen(true);
    }

    function openEdit(item) {
        setEditId(item.id);
        setForm({
            soort: item.soort ?? "",
            potmaat: item.potmaat ?? "",
            steellengte: item.steellengte ?? "",
            hoeveelheid: item.hoeveelheid ?? 1,
            minimumPrijs: item.minimumPrijs ?? 0.01,
            klokLocatie: item.klokLocatie ?? "Aalsmeer",
            leverDatum: item.leverDatum ? String(item.leverDatum).slice(0, 10) : "",
            beschrijving: item.beschrijving ?? "",
            foto: null,
        });
        setFormOpen(true);
    }

    async function submitForm(e) {
        e.preventDefault();

        if (!form.soort.trim()) return alert("Soort is verplicht.");
        if (!(form.potmaat?.trim() || form.steellengte?.trim())) return alert("Vul potmaat of steellengte in.");
        if (!form.leverDatum) return alert("Leverdatum is verplicht.");
        if (!KLOKLOCATIES.includes(String(form.klokLocatie))) return alert("Kies een geldige kloklocatie.");

        const dto = {
            ...form,
            leverDatum: form.leverDatum,
        };

        try {
            if (editId) await update(editId, dto);
            else await create(dto);
            setFormOpen(false);
        } catch {}
    }

    async function confirmDelete(id) {
        const ok = window.confirm("Weet je zeker dat je deze aanmelding wilt verwijderen?");
        if (!ok) return;
        try {
            await remove(id);
        } catch {}
    }

    const shownList = useMemo(() => {
        let list = filteredAanmeldingen;

        if (view === "resultaten") {
            if (statusFilter === "open") list = list.filter((x) => x.isVerkocht === false);
            if (statusFilter === "verkocht") list = list.filter((x) => x.isVerkocht === true);
            if (statusFilter === "all") list = list.filter((x) => x.isVerkocht === true);
        }

        return list;
    }, [filteredAanmeldingen, view, statusFilter]);

    const ui = {
        page: {
            display: "grid",
            gridTemplateColumns: "240px 1fr",
            minHeight: "100vh",
            background: "#fafafa",
            color: "#111827",
            fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif",
        },
        nav: { padding: 20, borderRight: "1px solid #eee", background: "#fff" },
        navItem: (active) => ({
            padding: "10px 12px",
            borderRadius: 10,
            marginBottom: 8,
            cursor: "pointer",
            background: active ? "#f3f4f6" : "transparent",
            border: "1px solid transparent",
        }),
        main: { padding: 24 },
        topbar: {
            display: "flex",
            gap: 12,
            alignItems: "center",
            justifyContent: "space-between",
            marginBottom: 18,
        },
        cardRow: {
            display: "grid",
            gridTemplateColumns: "repeat(4, minmax(0, 1fr))",
            gap: 12,
            marginBottom: 16,
        },
        card: (clickable) => ({
            background: "#fff",
            borderRadius: 12,
            border: "1px solid #eee",
            padding: 16,
            boxShadow: "0 6px 18px rgba(17,24,39,0.04)",
            cursor: clickable ? "pointer" : "default",
        }),
        h1: { fontSize: 20, fontWeight: 600, margin: 0 },
        sub: { fontSize: 12, color: "#6b7280", marginTop: 2 },
        big: { fontSize: 28, fontWeight: 600, marginTop: 8 },
        grid: { display: "grid", gridTemplateColumns: selected ? "1.6fr 1fr" : "1fr", gap: 12 },
        panel: {
            background: "#fff",
            borderRadius: 12,
            border: "1px solid #eee",
            padding: 16,
            boxShadow: "0 6px 18px rgba(17,24,39,0.04)",
        },
        table: { width: "100%", borderCollapse: "separate", borderSpacing: "0 10px" },
        th: { textAlign: "left", fontSize: 12, color: "#6b7280", fontWeight: 500, padding: "0 10px" },
        tr: { background: "#fff" },
        td: { padding: "12px 10px", borderTop: "1px solid #eee", borderBottom: "1px solid #eee" },
        tdFirst: { borderLeft: "1px solid #eee", borderTopLeftRadius: 12, borderBottomLeftRadius: 12 },
        tdLast: { borderRight: "1px solid #eee", borderTopRightRadius: 12, borderBottomRightRadius: 12 },
        badge: (sold) => ({
            display: "inline-block",
            padding: "4px 10px",
            borderRadius: 999,
            fontSize: 12,
            border: "1px solid #eee",
            background: sold ? "#ecfdf5" : "#f9fafb",
        }),
        btn: { padding: "10px 12px", borderRadius: 10, border: "1px solid #eee", background: "#fff", cursor: "pointer" },
        primaryBtn: {
            padding: "10px 12px",
            borderRadius: 10,
            border: "1px solid #e5e7eb",
            background: "#f3f4f6",
            cursor: "pointer",
            fontWeight: 600,
        },
        input: { padding: "10px 12px", borderRadius: 10, border: "1px solid #eee", background: "#fff", width: "100%" },
        row: { display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 },
        modalBackdrop: {
            position: "fixed",
            inset: 0,
            background: "rgba(17,24,39,0.35)",
            display: "grid",
            placeItems: "center",
            padding: 16,
            zIndex: 50,
        },
        modal: {
            width: "min(720px, 100%)",
            background: "#fff",
            borderRadius: 14,
            border: "1px solid #eee",
            padding: 16,
            boxShadow: "0 20px 60px rgba(17,24,39,0.18)",
        },
    };

    return (
        <div style={ui.page}>
            <aside style={ui.nav}>
                <div style={{ marginBottom: 18 }}>
                    <div style={{ fontWeight: 700 }}>Aanvoerder</div>
                    <div style={ui.sub}>Dashboard</div>
                </div>

                <div style={ui.navItem(view === "overzicht")} onClick={() => setView("overzicht")} role="button" tabIndex={0}>
                    Overzicht
                </div>
                <div style={ui.navItem(view === "aanmeldingen")} onClick={() => setView("aanmeldingen")} role="button" tabIndex={0}>
                    Aanmeldingen
                </div>
                <div style={ui.navItem(view === "resultaten")} onClick={() => setView("resultaten")} role="button" tabIndex={0}>
                    Resultaten
                </div>
                <div style={ui.navItem(view === "profiel")} onClick={() => setView("profiel")} role="button" tabIndex={0}>
                    Profiel / instellingen
                </div>
            </aside>

            <main style={ui.main}>
                <div style={ui.topbar}>
                    <div>
                        <h1 style={ui.h1}>
                            {view === "profiel" ? "Profiel" : "Aanvoerder Dashboard"}{" "}
                            <span style={{ color: "#6b7280", fontWeight: 500 }}>— {aanvoerderNaam}</span>
                        </h1>
                        <div style={ui.sub}>Rustig overzicht. Aanmeldingen beheren. Resultaten terugzien.</div>
                    </div>

                    {view !== "profiel" && (
                        <div style={{ display: "flex", gap: 10, alignItems: "center", minWidth: 560 }}>
                            <div style={{ width: 180 }}>
                                <input
                                    style={ui.input}
                                    type="date"
                                    value={leverdatum}
                                    onChange={(e) => setLeverdatum(e.target.value)}
                                    list="veildagenList"
                                    aria-label="Leverdatum filter"
                                    title="Leverdatum filter"
                                />
                                <datalist id="veildagenList">
                                    {(veildagen || []).map((d) => (
                                        <option key={d} value={d} />
                                    ))}
                                </datalist>
                            </div>

                            <input
                                style={ui.input}
                                type="text"
                                value={zoek}
                                onChange={(e) => setZoek(e.target.value)}
                                placeholder="Zoek op soort of beschrijving…"
                                aria-label="Zoeken"
                            />
                            <button style={ui.primaryBtn} onClick={openCreate} disabled={saving}>
                                Nieuwe aanmelding
                            </button>
                            <button style={ui.btn} onClick={() => alert("Uitloggen via je auth flow")} disabled={saving}>
                                Profiel ▾
                            </button>
                        </div>
                    )}
                </div>

                {error && (
                    <div style={{ ...ui.panel, borderColor: "#fee2e2", background: "#fff" }} role="alert">
                        <div style={{ fontWeight: 600 }}>Let op</div>
                        <div style={ui.sub}>{error}</div>
                    </div>
                )}

                {view === "profiel" ? (
                    <div style={ui.panel}>
                        <div style={{ fontWeight: 600, marginBottom: 8 }}>Profiel / instellingen</div>
                        <div style={ui.sub}>Dit scherm gebruikt je bestaande auth-profiel + logout flow.</div>
                    </div>
                ) : (
                    <>
                        <div style={ui.cardRow}>
                            <div style={ui.card(true)} onClick={() => setStatusFilter("all")} role="button" tabIndex={0}>
                                <div style={ui.sub}>Totaal aanmeldingen</div>
                                <div style={ui.big}>{totals.totaal}</div>
                            </div>

                            <div style={ui.card(true)} onClick={() => setStatusFilter("verkocht")} role="button" tabIndex={0}>
                                <div style={ui.sub}>Verkocht</div>
                                <div style={ui.big}>{totals.verkocht}</div>
                            </div>

                            <div style={ui.card(false)}>
                                <div style={ui.sub}>Totale opbrengst</div>
                                <div style={ui.big}>{money(totals.opbrengst)}</div>
                            </div>

                            <div style={ui.card(true)} onClick={() => setStatusFilter("open")} role="button" tabIndex={0}>
                                <div style={ui.sub}>Niet verkocht</div>
                                <div style={ui.big}>{totals.nietVerkocht}</div>
                            </div>
                        </div>

                        <div style={ui.grid}>
                            <div style={ui.panel}>
                                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 10 }}>
                                    <div>
                                        <div style={{ fontWeight: 600 }}>
                                            {view === "resultaten" ? "Resultaten" : "Aanmeldingen"} <span style={ui.sub}>({shownList.length})</span>
                                        </div>
                                        <div style={ui.sub}>Status komt uit backend: IsVerkocht (verkocht) anders open/niet verkocht.</div>
                                    </div>

                                    <div style={{ display: "flex", gap: 8 }}>
                                        <button style={ui.btn} onClick={() => setStatusFilter("all")}>
                                            Alles
                                        </button>
                                        <button style={ui.btn} onClick={() => setStatusFilter("verkocht")}>
                                            Verkocht
                                        </button>
                                        <button style={ui.btn} onClick={() => setStatusFilter("open")}>
                                            Open
                                        </button>
                                    </div>
                                </div>

                                {loading ? (
                                    <div style={ui.sub}>Laden…</div>
                                ) : (
                                    <table style={ui.table}>
                                        <thead>
                                        <tr>
                                            <th style={ui.th}>Foto</th>
                                            <th style={ui.th}>Soort</th>
                                            <th style={ui.th}>Potmaat</th>
                                            <th style={ui.th}>Steellengte</th>
                                            <th style={ui.th}>Hoeveelheid</th>
                                            <th style={ui.th}>Minimumprijs</th>
                                            <th style={ui.th}>Leverdatum</th>
                                            <th style={ui.th}>KlokLocatie</th>
                                            <th style={ui.th}>Status</th>
                                            <th style={ui.th}>Acties</th>
                                        </tr>
                                        </thead>
                                        <tbody>
                                        {shownList.map((a) => (
                                            <tr key={a.id} style={ui.tr} onClick={() => setSelected(a)} role="button" tabIndex={0}>
                                                <td style={{ ...ui.td, ...ui.tdFirst }}>
                                                    {a.fotoUrl ? (
                                                        <img
                                                            src={a.fotoUrl}
                                                            alt={a.soort}
                                                            style={{ width: 46, height: 46, borderRadius: 12, objectFit: "cover", border: "1px solid #eee" }}
                                                        />
                                                    ) : (
                                                        <div style={{ width: 46, height: 46, borderRadius: 12, background: "#f3f4f6", border: "1px solid #eee" }} />
                                                    )}
                                                </td>
                                                <td style={ui.td}>{a.soort}</td>
                                                <td style={ui.td}>{a.potmaat ?? "-"}</td>
                                                <td style={ui.td}>{a.steellengte ?? "-"}</td>
                                                <td style={ui.td}>{a.hoeveelheid}</td>
                                                <td style={ui.td}>{money(a.minimumPrijs)}</td>
                                                <td style={ui.td}>{dateNL(a.leverDatum)}</td>
                                                <td style={ui.td}>{a.klokLocatie}</td>
                                                <td style={ui.td}>
                                                    <span style={ui.badge(a.isVerkocht)}>{a.isVerkocht ? "Verkocht" : "Open"}</span>
                                                </td>
                                                <td style={{ ...ui.td, ...ui.tdLast }} onClick={(e) => e.stopPropagation()}>
                                                    <div style={{ display: "flex", gap: 8 }}>
                                                        <button style={ui.btn} onClick={() => openEdit(a)} disabled={saving}>
                                                            Bewerken
                                                        </button>
                                                        <button style={ui.btn} onClick={() => confirmDelete(a.id)} disabled={saving}>
                                                            Verwijderen
                                                        </button>
                                                    </div>
                                                </td>
                                            </tr>
                                        ))}

                                        {shownList.length === 0 && (
                                            <tr>
                                                <td colSpan={10} style={{ padding: 12, color: "#6b7280" }}>
                                                    Geen aanmeldingen gevonden.
                                                </td>
                                            </tr>
                                        )}
                                        </tbody>
                                    </table>
                                )}
                            </div>

                            {selected && (
                                <div style={ui.panel}>
                                    <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                                        <div style={{ fontWeight: 600 }}>Details</div>
                                        <button style={ui.btn} onClick={() => setSelected(null)}>
                                            Sluiten
                                        </button>
                                    </div>

                                    <div style={{ marginTop: 12, display: "grid", gap: 10 }}>
                                        {selected.fotoUrl && (
                                            <img
                                                src={selected.fotoUrl}
                                                alt={selected.soort}
                                                style={{ width: "100%", height: 180, borderRadius: 12, objectFit: "cover", border: "1px solid #eee" }}
                                            />
                                        )}

                                        <div>
                                            <div style={{ fontWeight: 600 }}>{selected.soort}</div>
                                            <div style={ui.sub}>Aanvoerder: {selected.aanvoerderNaam}</div>
                                        </div>

                                        <div style={ui.row}>
                                            <div>
                                                <div style={ui.sub}>Potmaat</div>
                                                <div>{selected.potmaat ?? "-"}</div>
                                            </div>
                                            <div>
                                                <div style={ui.sub}>Steellengte</div>
                                                <div>{selected.steellengte ?? "-"}</div>
                                            </div>
                                        </div>

                                        <div style={ui.row}>
                                            <div>
                                                <div style={ui.sub}>Hoeveelheid</div>
                                                <div>{selected.hoeveelheid}</div>
                                            </div>
                                            <div>
                                                <div style={ui.sub}>Minimumprijs</div>
                                                <div>{money(selected.minimumPrijs)}</div>
                                            </div>
                                        </div>

                                        <div style={ui.row}>
                                            <div>
                                                <div style={ui.sub}>Leverdatum</div>
                                                <div>{dateNL(selected.leverDatum)}</div>
                                            </div>
                                            <div>
                                                <div style={ui.sub}>KlokLocatie</div>
                                                <div>{selected.klokLocatie}</div>
                                            </div>
                                        </div>

                                        <div>
                                            <div style={ui.sub}>Beschrijving</div>
                                            <div>{selected.beschrijving ?? "-"}</div>
                                        </div>

                                        <div style={{ borderTop: "1px solid #eee", paddingTop: 10 }}>
                                            <div style={{ fontWeight: 600, marginBottom: 6 }}>Resultaat</div>
                                            <div style={ui.sub}>Status: {selected.isVerkocht ? "Verkocht" : "Open / niet verkocht"}</div>

                                            {selected.isVerkocht && (
                                                <div style={{ marginTop: 8, display: "grid", gap: 6 }}>
                                                    <div>
                                                        <span style={ui.sub}>Koper: </span>
                                                        {selected.koperNaam ?? "-"}
                                                    </div>
                                                    <div>
                                                        <span style={ui.sub}>Verkoopprijs: </span>
                                                        {money(selected.verkoopPrijs)}
                                                    </div>
                                                    <div>
                                                        <span style={ui.sub}>Totale opbrengst: </span>
                                                        {money(selected.totaleOpbrengst)}
                                                    </div>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            )}
                        </div>
                    </>
                )}
            </main>

            {formOpen && (
                <div style={ui.modalBackdrop} role="dialog" aria-modal="true">
                    <div style={ui.modal}>
                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                            <div style={{ fontWeight: 700 }}>{editId ? "Aanmelding bewerken" : "Nieuwe aanmelding"}</div>
                            <button style={ui.btn} onClick={() => setFormOpen(false)} disabled={saving}>
                                Sluiten
                            </button>
                        </div>

                        <form onSubmit={submitForm} style={{ marginTop: 12, display: "grid", gap: 12 }}>
                            <div style={ui.row}>
                                <div>
                                    <div style={ui.sub}>Soort *</div>
                                    <input style={ui.input} value={form.soort} onChange={(e) => setForm({ ...form, soort: e.target.value })} />
                                </div>
                                <div>
                                    <div style={ui.sub}>Leverdatum *</div>
                                    <input
                                        style={ui.input}
                                        type="date"
                                        value={form.leverDatum}
                                        onChange={(e) => setForm({ ...form, leverDatum: e.target.value })}
                                        list="veildagenList"
                                    />
                                </div>
                            </div>

                            <div style={ui.row}>
                                <div>
                                    <div style={ui.sub}>Potmaat (optioneel)</div>
                                    <input style={ui.input} value={form.potmaat} onChange={(e) => setForm({ ...form, potmaat: e.target.value })} />
                                </div>
                                <div>
                                    <div style={ui.sub}>Steellengte (optioneel)</div>
                                    <input
                                        style={ui.input}
                                        value={form.steellengte}
                                        onChange={(e) => setForm({ ...form, steellengte: e.target.value })}
                                    />
                                </div>
                            </div>

                            <div style={ui.row}>
                                <div>
                                    <div style={ui.sub}>Hoeveelheid *</div>
                                    <input
                                        style={ui.input}
                                        type="number"
                                        min="1"
                                        value={form.hoeveelheid}
                                        onChange={(e) => setForm({ ...form, hoeveelheid: Number(e.target.value) })}
                                    />
                                </div>
                                <div>
                                    <div style={ui.sub}>Minimumprijs *</div>
                                    <input
                                        style={ui.input}
                                        type="number"
                                        step="0.01"
                                        min="0.01"
                                        value={form.minimumPrijs}
                                        onChange={(e) => setForm({ ...form, minimumPrijs: Number(e.target.value) })}
                                    />
                                </div>
                            </div>

                            <div style={ui.row}>
                                <div>
                                    <div style={ui.sub}>KlokLocatie *</div>
                                    <select style={ui.input} value={form.klokLocatie} onChange={(e) => setForm({ ...form, klokLocatie: e.target.value })}>
                                        {KLOKLOCATIES.map((k) => (
                                            <option key={k} value={k}>
                                                {k}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div>
                                    <div style={ui.sub}>Foto (optioneel)</div>
                                    <input
                                        style={ui.input}
                                        type="file"
                                        accept="image/*"
                                        onChange={(e) => setForm({ ...form, foto: e.target.files?.[0] ?? null })}
                                    />
                                </div>
                            </div>

                            <div>
                                <div style={ui.sub}>Beschrijving (optioneel)</div>
                                <textarea
                                    style={{ ...ui.input, minHeight: 90 }}
                                    value={form.beschrijving}
                                    onChange={(e) => setForm({ ...form, beschrijving: e.target.value })}
                                />
                            </div>

                            <div style={{ display: "flex", justifyContent: "space-between", gap: 10, alignItems: "center" }}>
                                <div style={ui.sub}>
                                    Mogelijke backend fouten: weekend/feestdag, potmaat of steellengte, niet gevonden, geen aanvoerder-profiel.
                                </div>
                                <button style={ui.primaryBtn} type="submit" disabled={saving}>
                                    {saving ? "Opslaan…" : "Opslaan"}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
