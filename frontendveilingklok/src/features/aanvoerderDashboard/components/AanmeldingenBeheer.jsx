import React, { useState } from "react";
import { updateAanmelding, deleteAanmelding } from "../api/aanvoerderApi";

export default function AanmeldingenBeheer({ items, onClose, token, onUpdated }) {
    const [editing, setEditing] = useState(null);
    const [confirmDelete, setConfirmDelete] = useState(null);

    // Form state
    const [form, setForm] = useState({
        soort: "",
        potmaat: "",
        steellengte: "",
        hoeveelheid: "",
        minimumPrijs: "",
        klokLocatie: "Naaldwijk",
        veildatum: "",
        fotoUrl: "",
    });

    function startEdit(item) {
        setEditing(item.id);
        setForm({
            soort: item.soort,
            potmaat: item.potmaat || "",
            steellengte: item.steellengte || "",
            hoeveelheid: item.hoeveelheid,
            minimumPrijs: item.minimumPrijs,
            klokLocatie: item.klokLocatie,
            veildatum: item.veildatum.split("T")[0],
            fotoUrl: item.fotoUrl || "",
        });
    }

    async function saveEdit() {
        await updateAanmelding({
            token,
            id: editing,
            data: {
                ...form,
                hoeveelheid: Number(form.hoeveelheid),
                minimumPrijs: Number(form.minimumPrijs),
                veildatum: new Date(form.veildatum + "T00:00:00"),
            },
        });

        onUpdated(); // dashboard data opnieuw laden
        setEditing(null);
    }

    async function confirmDeleteAction() {
        await deleteAanmelding({ token, id: confirmDelete });
        onUpdated();
        setConfirmDelete(null);
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    }

    return (
        <div className="modal d-block" tabIndex="-1">
            <div className="modal-dialog modal-xl">
                <div className="modal-content">

                    <div className="modal-header">
                        <h5 className="modal-title">Aanmeldingen beheren</h5>
                        <button className="btn-close" onClick={onClose}></button>
                    </div>

                    <div className="modal-body">

                        <table className="table table-hover">
                            <thead>
                                <tr>
                                    <th>Soort</th>
                                    <th>Hoeveelheid</th>
                                    <th>Min. prijs</th>
                                    <th>Acties</th>
                                </tr>
                            </thead>

                            <tbody>
                                {items.map(a => (
                                    <tr key={a.id}>
                                        <td>{a.soort}</td>
                                        <td>{a.hoeveelheid}</td>
                                        <td>€{a.minimumPrijs.toFixed(2)}</td>

                                        <td>
                                            <button
                                                className="btn btn-sm btn-primary me-2"
                                                onClick={() => startEdit(a)}
                                            >
                                                Bewerken
                                            </button>

                                            <button
                                                className="btn btn-sm btn-danger"
                                                onClick={() => setConfirmDelete(a.id)}
                                            >
                                                Verwijderen
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>

                        {/* BEWERK MODAL */}
                        {editing && (
                            <div className="mt-4 p-3 border rounded bg-light">
                                <h5>Aanmelding bewerken</h5>

                                <div className="row g-2 mt-2">

                                    <div className="col-md-4">
                                        <label className="form-label">Soort</label>
                                        <input
                                            name="soort"
                                            className="form-control"
                                            value={form.soort}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Potmaat</label>
                                        <input
                                            name="potmaat"
                                            className="form-control"
                                            value={form.potmaat}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-4">
                                        <label className="form-label">Steellengte</label>
                                        <input
                                            name="steellengte"
                                            className="form-control"
                                            value={form.steellengte}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-3">
                                        <label className="form-label">Hoeveelheid</label>
                                        <input
                                            name="hoeveelheid"
                                            type="number"
                                            className="form-control"
                                            value={form.hoeveelheid}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-3">
                                        <label className="form-label">Minimumprijs (€)</label>
                                        <input
                                            name="minimumPrijs"
                                            type="number"
                                            className="form-control"
                                            value={form.minimumPrijs}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-md-6">
                                        <label className="form-label">Veildatum</label>
                                        <input
                                            name="veildatum"
                                            type="date"
                                            className="form-control"
                                            value={form.veildatum}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                    <div className="col-12">
                                        <label className="form-label">Foto-URL</label>
                                        <input
                                            name="fotoUrl"
                                            className="form-control"
                                            value={form.fotoUrl}
                                            onChange={handleFormChange}
                                        />
                                    </div>

                                </div>

                                <div className="d-flex justify-content-end gap-2 mt-3">
                                    <button
                                        className="btn btn-secondary"
                                        onClick={() => setEditing(null)}
                                    >
                                        Annuleren
                                    </button>

                                    <button
                                        className="btn btn-success"
                                        onClick={saveEdit}
                                    >
                                        Opslaan
                                    </button>
                                </div>
                            </div>
                        )}

                        {/* DELETE CONFIRMATION */}
                        {confirmDelete && (
                            <div className="alert alert-danger mt-4">
                                <h5>Weet je zeker dat je deze aanmelding wilt verwijderen?</h5>

                                <div className="d-flex justify-content-end gap-2 mt-2">
                                    <button
                                        className="btn btn-secondary"
                                        onClick={() => setConfirmDelete(null)}
                                    >
                                        Annuleren
                                    </button>

                                    <button
                                        className="btn btn-danger"
                                        onClick={confirmDeleteAction}
                                    >
                                        Verwijderen
                                    </button>
                                </div>
                            </div>
                        )}

                    </div>
                </div>
            </div>
        </div>
    );
}
