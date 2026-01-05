import { useContext, useEffect, useRef, useState } from "react";
import { AuthContext } from "../auth/AuthContext.jsx";
import Sidebar from "./components/Sidebar.jsx";

import {
    fetchAanmeldingen,
    createAanmelding,
    fetchAanvoerderStats,
} from "./api/aanvoerderApi.js";

import VeildagPicker from "./components/VeildagPicker.jsx";
import AanmeldingenBeheer from "./components/AanmeldingenBeheer.jsx";


export default function AanvoerderDashboard() {
    const { token, role, logout } = useContext(AuthContext);

    const fotoInputRef = useRef(null);

    // anchors
    const sectionNieuwRef = useRef(null);
    const sectionMijnRef = useRef(null);

    const [sidebarOpen, setSidebarOpen] = useState(false);

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
        beschrijving: "",
    });

    const [formError, setFormError] = useState("");
    const [formSuccess, setFormSuccess] = useState("");

    useEffect(() => {
        if (!token || role !== "Aanvoerder") return;
        loadDashboardData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [token, role, filterDate]);

    useEffect(() => {
        if (!formSuccess) return;

        const t = setTimeout(() => {
            setFormSuccess("");
        }, 3000);

        return () => clearTimeout(t);
    }, [formSuccess]);

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

        if (formSuccess) setFormSuccess("");
        if (formError) setFormError("");

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
            leverdatum: form.leverdatum.toISOString(),
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
                beschrijving: "",
            });

            if (fotoInputRef.current) {
                fotoInputRef.current.value = "";
            }
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

    function handleSidebarNavigate(target) {
        setSidebarOpen(false);

        if (target === "dashboard") {
            window.scrollTo({ top: 0, behavior: "smooth" });
            return;
        }

        if (target === "nieuw") {
            sectionNieuwRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
            return;
        }

        if (target === "mijn") {
            sectionMijnRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
            return;
        }

        if (target === "beheer") {
            setBeheerOpen(true);
            return;
        }
    }

    function handleLogout() {
        // Als je AuthContext een logout() aanbiedt is dit genoeg.
        // Zo niet: token uit localStorage verwijderen + redirect.
        if (logout) logout();
        else {
            localStorage.removeItem("token");
            window.location.href = "/login";
        }
    }

    return (
        <main id="main" className="container py-4">
            <Sidebar
                open={sidebarOpen}
                onClose={() => setSidebarOpen(false)}
                onNavigate={handleSidebarNavigate}
                onLogout={handleLogout}
                userLabel="Aanvoerder"
            />

            <div className="d-flex justify-content-between align-items-center mb-4">
                <div className="d-flex align-items-center gap-2">
                    <button className="btn btn-outline-secondary" onClick={() => setSidebarOpen(true)}>
                        Menu
                    </button>
                    <h1 className="h3 mb-0">Aanvoerdersdashboard</h1>
                </div>
                <p className="text-muted mb-0">Beheer je veilingaanmeldingen</p>
            </div>

            {stats && (
                <section className="mb-4">
                    <div className="row g-3">
                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Totaal aanmeldingen</p>
                                    <p className="fs-4 fw-bold">{stats.totaalAantalAanmeldingen}</p>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Verkocht</p>
                                    <p className="fs-4 fw-bold">{stats.aantalVerkocht}</p>
                                </div>
                            </div>
                        </div>

                        <div className="col-md-4">
                            <div className="card shadow-sm border-0">
                                <div className="card-body">
                                    <p className="text-muted mb-1">Totale opbrengst</p>
                                    <p className="fs-4 fw-bold">{stats.totaleOpbrengst.toFixed(2)}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            )}

            <section className="mb-5" ref={sectionNieuwRef}>
                <div className="card shadow-sm border-0">
                    <div className="card-body">
                        <h2 className="h4 mb-3">Nieuw product aanmelden</h2>

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
                                        onChange={(value) => {
                                            if (formSuccess) setFormSuccess("");
                                            if (formError) setFormError("");
                                            setForm((prev) => ({ ...prev, leverdatum: value }));
                                        }}
                                        highlightedDates={items.map((i) => i.leverDatum)}
                                    />
                                </div>

                                <div className="col-md-8">
                                    <label className="form-label">Productfoto</label>
                                    <input
                                        ref={fotoInputRef}
                                        type="file"
                                        accept="image/*"
                                        className="form-control"
                                        onChange={(e) => {
                                            if (formSuccess) setFormSuccess("");
                                            if (formError) setFormError("");
                                            const file = e.target.files?.[0] ?? null;
                                            setForm((prev) => ({ ...prev, fotoFile: file }));
                                        }}
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
                                <button type="submit" className="btn btn-success">
                                    Aanmelden
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </section>

            <section ref={sectionMijnRef}>
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h2 className="h4">Mijn aanmeldingen</h2>

                    <button className="btn btn-outline-primary" onClick={() => setBeheerOpen(true)}>
                        Aanmeldingen beheren
                    </button>
                </div>

                <div className="d-flex gap-2 mb-3">
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
                {loading && <p>Laden...</p>}

                {!loading && filteredItems.length === 0 && <p>Geen aanmeldingen.</p>}

                {!loading && filteredItems.length > 0 && (
                    <div className="table-responsive">
                        <table className="table table-hover">
                            <thead className="table-light">
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
                                            {item.fotoUrl ? (
                                                <img
                                                    src={item.fotoUrl}
                                                    style={{
                                                        width: "64px",
                                                        height: "64px",
                                                        objectFit: "cover",
                                                        borderRadius: "8px",
                                                    }}
                                                    alt={item.soort}
                                                />
                                            ) : (
                                                <span className="text-muted">Geen foto</span>
                                            )}
                                        </td>

                                        <td>{item.soort}</td>
                                        <td>{item.potmaat || item.steellengte || "-"}</td>
                                        <td>{item.hoeveelheid}</td>
                                        <td>{item.minimumPrijs.toFixed(2)}</td>
                                        <td>{item.klokLocatie}</td>
                                        <td>{new Date(item.leverDatum).toLocaleDateString("nl-NL")}</td>
                                        <td>{item.aanvoerderNaam}</td>

                                        <td>
                                            {item.isVerkocht ? (
                                                <>
                                                    <div>{item.verkoopPrijs?.toFixed(2)} / stuk</div>
                                                    <div className="small text-muted">
                                                        Totaal: {item.totaleOpbrengst?.toFixed(2)}
                                                        {item.koperNaam && <> – {item.koperNaam}</>}
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
    );
}
