import React from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

// Feestdagen toevoegen
const FEESTDAGEN = [
    "2025-01-01", // Nieuwjaarsdag
    "2025-04-18", // Goede Vrijdag
    "2025-04-20", // Pasen
    "2025-04-21", // Tweede Paasdag
    "2025-05-29", // Hemelvaartsdag
    "2025-06-08", // Pinksteren
    "2025-06-09", // Tweede Pinksterdag
    "2025-12-25", // Kerst
    "2025-12-26"  // Tweede Kerstdag
];

export default function VeildagPicker({ value, onChange, highlightedDates = [] }) {
    const selectedDate = value ? new Date(value) : null;

    // disable weekend + feestdagen
    const isDayBlocked = (date) => {
        const iso = date.toISOString().split("T")[0];

        // weekend
        if (date.getDay() === 0 || date.getDay() === 6) return true;

        // feestdagen
        if (FEESTDAGEN.includes(iso)) return true;

        return false;
    };

    // highlight datums waar al producten op staan
    const highlight = highlightedDates.map((d) => new Date(d));

    return (
        <DatePicker
            selected={selectedDate}
            onChange={(date) => {
                if (!date) return;
                const iso = date.toISOString().substring(0, 10);
                onChange(iso);
            }}
            className="form-control"
            placeholderText="Kies een veildatum"
            dateFormat="yyyy-MM-dd"
            filterDate={(date) => !isDayBlocked(date)}
            highlightDates={highlight}
            popperPlacement="bottom-start"
            wrapperClassName="w-100"
        />

    );
}
