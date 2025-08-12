import { useEffect, useMemo, useState } from "react";
import type { EmployeeDto } from "../dtos/EmployeeDto";
import "../styles/metricell.css";

interface Props {
  rows: EmployeeDto[];
  onEdit: (row: EmployeeDto) => void;
  onDelete: (id: number) => void;
}

/**
 * EmployeesTable
 * Metricell-styled table with search, sort, and client-side pagination (no vertical scroll).
 */
export default function EmployeesTable({ rows, onEdit, onDelete }: Props) {
  const [query, setQuery] = useState("");
  const [sortAsc, setSortAsc] = useState<boolean>(true);

  // Pagination state
  const [page, setPage] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);
  const pageSizes = [10, 25, 50];

  // Filter + sort
  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    const base = q
      ? rows.filter((r) => r.name.toLowerCase().includes(q))
      : rows.slice();
    return base.sort((a, b) =>
      sortAsc ? a.name.localeCompare(b.name) : b.name.localeCompare(a.name)
    );
  }, [rows, query, sortAsc]);

  // Compute pages
  const totalItems = filtered.length;
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));

  // Keep page within bounds when filters/pageSize change
  useEffect(() => {
    setPage((p) => Math.min(Math.max(1, p), totalPages));
  }, [totalPages]);

  const paged = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page, pageSize]);

  const prev = () => setPage((p) => Math.max(1, p - 1));
  const next = () => setPage((p) => Math.min(totalPages, p + 1));

  return (
    <div className="card" style={{ padding: 12 }}>
      <div className="flex items-center justify-between mb-8 gap-8">
        <h3 style={{ margin: 0 }}>Employees ({rows.length})</h3>
        <div className="flex items-center gap-8">
          <input
            className="input"
            aria-label="Search employees by name"
            placeholder="Search by name"
            value={query}
            onChange={(e) => {
              setQuery(e.target.value);
              setPage(1);
            }}
          />
          <button
            type="button"
            className="btn btn--outline"
            onClick={() => setSortAsc((s) => !s)}
            aria-label="Toggle sort order"
          >
            Sort: {sortAsc ? "A→Z" : "Z→A"}
          </button>
        </div>
      </div>

      <table className="table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th className="text-right">Value</th>
            <th className="text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          {paged.map((r) => (
            <tr key={r.id}>
              <td>{r.id}</td>
              <td>{r.name}</td>
              <td className="text-right">{r.value.toLocaleString()}</td>
              <td className="text-right">
                <button
                  type="button"
                  className="btn btn--outline"
                  onClick={() => onEdit(r)}
                  style={{ marginRight: 8 }}
                >
                  Edit
                </button>
                <button
                  type="button"
                  className="btn btn--danger"
                  onClick={() => onDelete(r.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}

          {paged.length === 0 && (
            <tr>
              <td
                colSpan={4}
                style={{ padding: 12, textAlign: "center" }}
                className="text-muted"
              >
                No employees found
              </td>
            </tr>
          )}
        </tbody>
      </table>

      {/* Pagination controls */}
      <div className="flex items-center justify-between mt-8">
        <div className="text-muted">
          Showing <strong>{paged.length}</strong> of{" "}
          <strong>{totalItems}</strong>
        </div>

        <div className="flex items-center gap-8">
          <label className="text-muted">
            Page size
            <select
              className="input"
              value={pageSize}
              onChange={(e) => {
                setPageSize(Number(e.target.value));
                setPage(1);
              }}
              style={{ width: 90, display: "inline-block", marginLeft: 6 }}
            >
              {pageSizes.map((n) => (
                <option key={n} value={n}>
                  {n}
                </option>
              ))}
            </select>
          </label>

          <div className="text-muted">
            Page <strong>{page}</strong> of <strong>{totalPages}</strong>
          </div>

          <div className="flex items-center gap-8">
            <button
              className="btn btn--outline"
              onClick={prev}
              disabled={page === 1}
            >
              Prev
            </button>
            <button
              className="btn btn--primary"
              onClick={next}
              disabled={page === totalPages}
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
