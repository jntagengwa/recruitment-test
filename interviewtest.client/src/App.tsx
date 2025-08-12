import { useEffect, useMemo, useState } from "react";
import { EmployeesApi } from "./api/Employees";
import type { EmployeeDto } from "./dtos/EmployeeDto";
import type { EmployeeCreateDto } from "./dtos/EmployeeCreateDto";
import type { EmployeeUpdateDto } from "./dtos/EmployeeUpdateDto";
import EmployeesTable from "./components/EmployeesTable";
import EmployeesForm from "./components/EmployeesForm";

function App() {
  const [rows, setRows] = useState<EmployeeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>("");

  const [editing, setEditing] = useState<EmployeeDto | null>(null);
  const [abcSum, setAbcSum] = useState<number | null>(null);

  const total = useMemo(
    () => rows.reduce((acc, r) => acc + r.value, 0),
    [rows]
  );

  useEffect(() => {
    refresh();
  }, []);

  async function refresh() {
    setLoading(true);
    setError("");
    try {
      const data = await EmployeesApi.list();
      setRows(data);
    } catch (e: unknown) {
      const msg = e instanceof Error ? e.message : "Failed to load employees";
      setError(msg);
    } finally {
      setLoading(false);
    }
  }

  function beginCreate() {
    setEditing(null);
  }

  function beginEdit(row: EmployeeDto) {
    setEditing(row);
  }

  async function handleSave(data: EmployeeCreateDto | EmployeeUpdateDto) {
    try {
      const payload = { name: data.name.trim(), value: Number(data.value) };

      if (editing) {
        // Update existing
        await EmployeesApi.update(editing.id, payload);
        setRows((prev) =>
          prev.map((r) => (r.id === editing.id ? { ...r, ...payload } : r))
        );
        setEditing(null);
      } else {
        // Create new
        const created = await EmployeesApi.create(payload);
        setRows((prev) => [...prev, created]);
      }
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Save failed";
      alert(msg);
    }
  }

  async function handleDelete(id: number) {
    if (!confirm("Delete this employee?")) return;
    try {
      await EmployeesApi.remove(id);
      setRows((prev) => prev.filter((r) => r.id !== id));
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Delete failed";
      alert(msg);
    }
  }

  async function runRule() {
    try {
      const res = await EmployeesApi.runRule();
      setAbcSum(res ? res.sumOfABC : null);
      await refresh();
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Operation failed";
      alert(msg);
    }
  }

  return (
    <div
      className="app-container"
      style={{ maxWidth: 960, margin: "24px auto", padding: 16 }}
    >
      <header
        className="app-header"
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: 24,
        }}
      >
        <h2 className="app-title" style={{ margin: 0 }}>
          Metricell Employees
        </h2>
        <div className="app-actions" style={{ display: "flex", gap: 12 }}>
          <button className="btn btn--primary" onClick={beginCreate}>
            New Employee
          </button>
          <button className="btn btn--accent" onClick={runRule}>
            Run Increment Rule
          </button>
          <button className="btn" onClick={refresh}>
            Refresh
          </button>
        </div>
      </header>

      {abcSum !== null && (
        <div
          className="notification notification--success"
          style={{
            background: "#eef9f0",
            border: "1px solid #b0e6bd",
            color: "#155724",
            padding: 12,
            borderRadius: 6,
            marginBottom: 16,
          }}
        >
          Sum of A/B/C is <strong>{abcSum.toLocaleString()}</strong>
        </div>
      )}

      {error && (
        <div
          className="notification notification--error"
          style={{
            background: "#fdecea",
            border: "1px solid #f5c6cb",
            color: "#721c24",
            padding: 12,
            borderRadius: 6,
            marginBottom: 16,
          }}
        >
          {error}
        </div>
      )}

      <section
        className="content-section"
        style={{
          display: "grid",
          gridTemplateColumns: "minmax(280px, 360px) 1fr",
          gap: 24,
          alignItems: "start",
        }}
      >
        <EmployeesForm
          key={editing ? editing.id : "new"}
          initialValues={editing ?? undefined}
          onSubmit={handleSave}
          onCancel={() => setEditing(null)}
        />

        <div>
          <div
            className="card card--summary"
            style={{
              padding: 16,
              marginBottom: 16,
              backgroundColor: "#f9f9f9",
              borderRadius: 6,
            }}
          >
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                color: "#555",
                fontWeight: "600",
              }}
            >
              <span>Total Value:</span>
              <span>{total.toLocaleString()}</span>
            </div>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                color: "#555",
                fontWeight: "600",
                marginTop: 8,
              }}
            >
              <span>Count:</span>
              <span>{rows.length}</span>
            </div>
          </div>

          {loading ? (
            <div className="loading">Loading…</div>
          ) : (
            <EmployeesTable
              rows={rows}
              onEdit={beginEdit}
              onDelete={handleDelete}
            />
          )}
        </div>
      </section>
    </div>
  );
}

export default App;
