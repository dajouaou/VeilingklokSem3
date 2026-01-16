// AanvoerderDashboard.jsx (mooier layout + classes, zelfde logica)
import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../auth/AuthContext.jsx";
import placeholderImg from "../../Images/icon.jpg";


import {
    fetchAanmeldingen,
    createAanmelding,
    fetchAanvoerderStats,
} from "./api/aanvoerderApi.js";

import VeildagPicker from "./components/VeildagPicker.jsx";
import AanmeldingenBeheer from "./components/AanmeldingenBeheer.jsx";
import AanvoerderNavbar from "./components/AanvoerderNavbar";

import "./AanvoerderDashboard.css";

export default function AanvoerderDashboard() {
    const { token, role } = useContext(AuthContext);

    function toYmd(date) {
        const y = date.getFullYear();
        const m = String(date.getMonth() + 1).padStart(2, "0");
        const d = String(date.getDate()).padStart(2, "0");
        return `${y}-${m}-${d}`;
    }

    const [items, setItems] = useState([]);
    const [stats, setStats] = useState(null);

    const [loading, setLoading] = useState(false);
    const [loadError, setLoadError] = useState("");

    const [filterDate, setFilterDate] = useState("");
    const [search, setSearch] = useState("");
    const [beheerOpen, setBeheerOpen] = useState(false);

    const [form, setForm] = useState({
        soort: "",
        potmaat: "",
        steellengte: "",
        hoeveelheid: "",
        minimumPrijs: "",
        klokLocatie: "Naaldwijk",
        leverdatum: null,
        fotoFile: null,
    });

    const [formError, setFormError] = useState("");
    const [formSuccess, setFormSuccess] = useState("");

    useEffect(() => {
        if (!token || role !== "Aanvoerder") return;
        loadDashboardData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [token, role, filterDate]);

    async function loadDashboardData() {
        setLoading(true);
        setLoadError("");

        try {
            const [aanmeldingen, statsDto] = await Promise.all([
                fetchAanmeldingen({ token, leverdatum: filterDate || undefined }),
                fetchAanvoerderStats({ token, leverdatum: filterDate || undefined }),
            ]);
            setItems(aanmeldingen);
            setStats(statsDto);
        } catch (err) {
            setLoadError(err.message);
        } finally {
            setLoading(false);
        }
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setFormError("");
        setFormSuccess("");

        if (!form.soort || !form.hoeveelheid || !form.minimumPrijs || !form.leverdatum) {
            setFormError("Vul minimaal soort, hoeveelheid, minimumprijs en leverdatum in.");
            return;
        }

        const payload = {
            soort: form.soort,
            potmaat: form.potmaat || null,
            steellengte: form.steellengte || null,
            hoeveelheid: Number(form.hoeveelheid),
            minimumPrijs: Number(form.minimumPrijs),
            klokLocatie: form.klokLocatie,
            leverdatum: toYmd(form.leverdatum),
            fotoFile: form.fotoFile,
            beschrijving: form.beschrijving || "",
        };

        try {
            const created = await createAanmelding({ token, data: payload });
            setItems((prev) => [...prev, created]);

            const updatedStats = await fetchAanvoerderStats({
                token,
                leverdatum: filterDate || undefined,
            });
            setStats(updatedStats);

            setFormSuccess("Product succesvol aangemeld.");
            setForm({
                soort: "",
                potmaat: "",
                steellengte: "",
                hoeveelheid: "",
                minimumPrijs: "",
                klokLocatie: "Naaldwijk",
                leverdatum: null,
                fotoFile: null,
            });
        } catch (err) {
            setFormError(err.message);
        }
    }

    const filteredItems = items.filter((item) => {
        if (!search) return true;
        const term = search.toLowerCase();
        return (
            item.soort.toLowerCase().includes(term) ||
            (item.potmaat?.toLowerCase?.() || "").includes(term) ||
            (item.steellengte?.toLowerCase?.() || "").includes(term)
        );
    });

    return (
        <>
            <AanvoerderNavbar />

            <main id="main" className="container py-4 av-page">
                {/* Header */}
                <div className="av-header">
                    <div>
                        <h1 className="h3 av-title">Aanvoerdersdashboard</h1>
                        <p className="av-sub">Beheer je veilingaanmeldingen</p>
                    </div>

                    <div className="av-chiprow">
                        <span className={`av-chip ${loading ? "primary" : "success"}`}>
                            <span className="dot" />
                            {loading ? "Laden..." : "Alles up-to-date"}
                        </span>

                        {filterDate && (
                            <span className="av-chip">
                                <span className="dot" />
                                Filter: <strong>{filterDate}</strong>
                            </span>
                        )}
                    </div>
                </div>

                {/* Hero */}
                <section className="av-hero">
                    <div className="d-flex flex-wrap justify-content-between align-items-center gap-2">
                        <div>
                            <h2 className="h5 mb-1 av-card-title">Snelle acties</h2>
                            <p className="text-muted mb-0">
                                Meld producten aan en beheer je aanmeldingen.
                            </p>
                        </div>

                        <button
                            className="btn av-btn-outline"
                            onClick={() => setBeheerOpen(true)}
                        >
                            Aanmeldingen beheren
                        </button>
                    </div>
                </section>

                {/* Stats */}
                {stats && (
                    <section className="mb-4 av-stats">
                        <div className="row g-3">
                            <div className="col-md-4">
                                <div className="card border-0">
                                    <div className="card-body">
                                        <div className="av-stat-label">Totaal aanmeldingen</div>
                                        <div className="fs-3 av-stat-value">
                                            {stats.totaalAantalAanmeldingen}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className="col-md-4">
                                <div className="card border-0">
                                    <div className="card-body">
                                        <div className="av-stat-label">Verkocht</div>
                                        <div className="fs-3 av-stat-value">{stats.aantalVerkocht}</div>
                                    </div>
                                </div>
                            </div>

                            <div className="col-md-4">
                                <div className="card border-0">
                                    <div className="card-body">
                                        <div className="av-stat-label">Totale opbrengst</div>
                                        <div className="fs-3 av-stat-value">
                                            {stats.totaleOpbrengst.toFixed(2)} EUR
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                )}

                {/* Form */}
                <section className="mb-5">
                    <div className="card border-0 av-card">
                        <div className="card-body">
                            <div className="d-flex justify-content-between align-items-start gap-3 mb-3">
                                <div>
                                    <h2 className="h4 mb-1 av-card-title">Nieuw product aanmelden</h2>
                                    <p className="text-muted mb-0">
                                        Vul de gegevens in en voeg optioneel een foto toe.
                                    </p>
                                </div>
                                <span className="av-chip success">
                                    <span className="dot" />
                                    Veilig opslaan
                                </span>
                            </div>

                            {formError && <div className="alert alert-danger">{formError}</div>}
                            {formSuccess && <div className="alert alert-success">{formSuccess}</div>}

                            <form onSubmit={handleSubmit}>
                                <div className="row g-3">
                                    <div className="col-md-6">
                                        <label className="form-label">Soort *</label>
                                        <input
                                            name="soort"
                                            type="text"
                                            className="form-control"
                                            value={form.soort}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-6">
                                        <label className="form-label">Potmaat</label>
                                        <input
                                            name="potmaat"
                                            type="text"
                                            className="form-control"
                                            value={form.potmaat}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-6">
                                        <label className="form-label">Steellengte</label>
                                        <input
                                            name="steellengte"
                                            type="text"
                                            className="form-control"
                                            value={form.steellengte}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Hoeveelheid *</label>
                                        <input
                                            name="hoeveelheid"
                                            type="number"
                                            min="1"
                                            className="form-control"
                                            value={form.hoeveelheid}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Minimumprijs (euro) *</label>
                                        <input
                                            name="minimumPrijs"
                                            type="number"
                                            step="0.01"
                                            min="0"
                                            className="form-control"
                                            value={form.minimumPrijs}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Kloklocatie</label>
                                        <select
                                            name="klokLocatie"
                                            className="form-select"
                                            value={form.klokLocatie}
                                            onChange={handleFormChange}
                                        >
                                            <option value="Naaldwijk">Naaldwijk</option>
                                            <option value="Aalsmeer">Aalsmeer</option>
                                            <option value="Rijnsburg">Rijnsburg</option>
                                            <option value="Eelde">Eelde</option>
                                        </select>
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Leverdatum *</label>
                                        <VeildagPicker
                                            value={form.leverdatum}
                                            onChange={(value) =>
                                                setForm((prev) => ({ ...prev, leverdatum: value }))
                                            }
                                            highlightedDates={items.map((i) => i.leverDatum ?? i.leverdatum)}
                                        />
                                    </div>

                                    <div className="col-md-8">
                                        <label className="form-label">Productfoto</label>
                                        <input
                                            type="file"
                                            accept="image/*"
                                            className="form-control"
                                            onChange={(e) =>
                                                setForm((prev) => ({ ...prev, fotoFile: e.target.files[0] }))
                                            }
                                        />
                                    </div>

                                    <div className="col-12">
                                        <label className="form-label">Beschrijving</label>
                                        <textarea
                                            name="beschrijving"
                                            className="form-control"
                                            rows="3"
                                            value={form.beschrijving || ""}
                                            onChange={handleFormChange}
                                        />
                                    </div>
                                </div>

                                <div className="mt-4 d-flex justify-content-end">
                                    <button type="submit" className="btn av-btn-success">
                                        Aanmelden
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </section>

                {/* List */}
                <section>
                    <div className="d-flex flex-wrap justify-content-between align-items-end gap-2 mb-3">
                        <div>
                            <h2 className="h4 mb-1 av-card-title">Mijn aanmeldingen</h2>
                            <p className="text-muted mb-0">Filter en zoek in je producten.</p>
                        </div>

                        <button className="btn av-btn-outline" onClick={() => setBeheerOpen(true)}>
                            Aanmeldingen beheren
                        </button>
                    </div>

                    <div className="av-filters mb-3">
                        <div>
                            <label className="form-label mb-1">Filter op leverdatum</label>
                            <input
                                type="date"
                                className="form-control"
                                value={filterDate}
                                onChange={(e) => setFilterDate(e.target.value)}
                            />
                        </div>

                        <div>
                            <label className="form-label mb-1">Zoek op soort</label>
                            <input
                                type="search"
                                className="form-control"
                                placeholder="Bijv. Rozen"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </div>
                    </div>

                    {loadError && <p className="text-danger">{loadError}</p>}
                    {loading && <p className="text-muted">Laden...</p>}

                    {!loading && filteredItems.length === 0 && (
                        <div className="alert alert-success">
                            Geen aanmeldingen gevonden voor je filters.
                        </div>
                    )}

                    {!loading && filteredItems.length > 0 && (
                        <div className="table-responsive av-tablewrap">
                            <table className="table table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th>Foto</th>
                                        <th>Soort</th>
                                        <th>Kenmerken</th>
                                        <th>Hoeveelheid</th>
                                        <th>Min. prijs</th>
                                        <th>Kloklocatie</th>
                                        <th>Leverdatum</th>
                                        <th>Aanvoerder</th>
                                        <th>Verkoop</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {filteredItems.map((item) => (
                                        <tr key={item.id}>
                                            <td>
                                                <img
                                                className="av-img"
                                                src={item.fotoUrl || placeholderImg}
                                                alt={item.soort}
                                                onError={(e) => {
                                                    e.currentTarget.src = placeholderImg;
                                                }}
                                            />


                                            </td>

                                            <td className="fw-semibold">{item.soort}</td>
                                            <td>{item.potmaat || item.steellengte || "-"}</td>
                                            <td>{item.hoeveelheid}</td>
                                            <td>{item.minimumPrijs.toFixed(2)} EUR</td>
                                            <td>{item.klokLocatie}</td>
                                            <td>{new Date(item.leverDatum).toLocaleDateString("nl-NL")}</td>
                                            <td>{item.aanvoerderNaam}</td>

                                            <td>
                                                {item.isVerkocht ? (
                                                    <>
                                                        <div className="fw-semibold">
                                                            {item.verkoopPrijs?.toFixed(2)} EUR / stuk
                                                        </div>
                                                        <div className="small text-muted">
                                                            Totaal: {item.totaleOpbrengst?.toFixed(2)} EUR
                                                            {item.koperNaam && <> {" \u2013 "} {item.koperNaam}</>}
                                                        </div>
                                                    </>
                                                ) : (
                                                    <span className="badge bg-secondary">Nog niet verkocht</span>
                                                )}
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </section>

                {beheerOpen && (
                    <AanmeldingenBeheer
                        items={filteredItems}
                        token={token}
                        onClose={() => setBeheerOpen(false)}
                        onUpdated={loadDashboardData}
                    />
                )}
            </main>
        </>
    );
}
