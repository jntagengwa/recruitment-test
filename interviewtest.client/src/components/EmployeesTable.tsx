import { useMemo, useState } from "react";
import type { EmployeeDto } from "../dtos/EmployeeDto";

interface Props {
  rows: EmployeeDto[];
  onEdit: (row: EmployeeDto) => void;
  onDelete: (id: number) => void;
}

/**
 * EmployeesTable
 * Simple, accessible table with a name filter and row actions.
 */
export default function EmployeesTable({ rows, onEdit, onDelete }: Props) {
  const [query, setQuery] = useState("");
  const [sortAsc, setSortAsc] = useState<boolean>(true);

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    const base = q
      ? rows.filter((r) => r.name.toLowerCase().includes(q))
      : rows.slice();
    return base.sort((a, b) =>
      sortAsc ? a.name.localeCompare(b.name) : b.name.localeCompare(a.name)
    );
  }, [rows, query, sortAsc]);

  return (
    <div className="card" style={{ padding: 12 }}>
      <div
        style={{
          display: "flex",
          gap: 8,
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: 8,
        }}
      >
        <h3 style={{ margin: 0 }}>Employees ({rows.length})</h3>
        <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
          <input
            aria-label="Search employees by name"
            placeholder="Search by name"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
          <button
            type="button"
            onClick={() => setSortAsc((s) => !s)}
            aria-label="Toggle sort order"
          >
            Sort: {sortAsc ? "A→Z" : "Z→A"}
          </button>
        </div>
      </div>

      <table style={{ width: "100%", borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th
              style={{
                textAlign: "left",
                borderBottom: "1px solid #e5e7eb",
                padding: "6px 4px",
              }}
            >
              ID
            </th>
            <th
              style={{
                textAlign: "left",
                borderBottom: "1px solid #e5e7eb",
                padding: "6px 4px",
              }}
            >
              Name
            </th>
            <th
              style={{
                textAlign: "right",
                borderBottom: "1px solid #e5e7eb",
                padding: "6px 4px",
              }}
            >
              Value
            </th>
            <th
              style={{
                textAlign: "right",
                borderBottom: "1px solid #e5e7eb",
                padding: "6px 4px",
              }}
            >
              Actions
            </th>
          </tr>
        </thead>
        <tbody>
          {filtered.map((r) => (
            <tr key={r.id}>
              <td
                style={{
                  padding: "8px 4px",
                  borderBottom: "1px solid #f1f5f9",
                }}
              >
                {r.id}
              </td>
              <td
                style={{
                  padding: "8px 4px",
                  borderBottom: "1px solid #f1f5f9",
                }}
              >
                {r.name}
              </td>
              <td
                style={{
                  padding: "8px 4px",
                  textAlign: "right",
                  borderBottom: "1px solid #f1f5f9",
                }}
              >
                {r.value.toLocaleString()}
              </td>
              <td
                style={{
                  padding: "8px 4px",
                  textAlign: "right",
                  borderBottom: "1px solid #f1f5f9",
                }}
              >
                <button
                  type="button"
                  onClick={() => onEdit(r)}
                  style={{ marginRight: 8 }}
                >
                  Edit
                </button>
                <button type="button" onClick={() => onDelete(r.id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}

          {filtered.length === 0 && (
            <tr>
              <td
                colSpan={4}
                style={{ padding: 12, textAlign: "center", color: "#6b7280" }}
              >
                No employees found
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
