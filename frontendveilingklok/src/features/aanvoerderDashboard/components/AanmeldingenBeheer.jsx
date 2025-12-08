import React from "react";

export default function AanmeldingenBeheer({ items, onClose }) {
    return (
        <div className="modal d-block" tabIndex="-1">
            <div className="modal-dialog modal-lg">
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
                                            <button className="btn btn-sm btn-primary me-2">
                                                Bewerken
                                            </button>
                                            <button className="btn btn-sm btn-danger">
                                                Verwijderen
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                </div>
            </div>
        </div>
    );
}
