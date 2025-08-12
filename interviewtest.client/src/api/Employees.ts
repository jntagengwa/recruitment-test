import type { EmployeeDto } from "../dtos/EmployeeDto";
import type { EmployeeCreateDto } from "../dtos/EmployeeCreateDto";
import type { EmployeeUpdateDto } from "../dtos/EmployeeUpdateDto";
import type { AbcSumResponse } from "../dtos/AbcSumResponse";

const JSON_HEADERS = { "Content-Type": "application/json" } as const;

async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) {
    // Try to surface server-provided problem details when available
    let body: unknown = null;
    try {
      body = await res.json();
    } catch {
      /* ignore */
    }
    let message: string;
    if (typeof body === "object" && body !== null) {
      const record = body as Record<string, unknown>;
      message =
        (typeof record.title === "string" && record.title) ||
        (typeof record.detail === "string" && record.detail) ||
        (typeof record.message === "string" && record.message) ||
        `${res.status} ${res.statusText}`;
    } else {
      message = `${res.status} ${res.statusText}`;
    }
    throw new Error(message);
  }
  // 204 No Content
  if (res.status === 204) return undefined as unknown as T;
  return (await res.json()) as T;
}

export const EmployeesApi = {
  /** List all employees */
  async list(): Promise<EmployeeDto[]> {
    const res = await fetch("/api/employees", { credentials: "include" });
    return handle<EmployeeDto[]>(res);
  },

  /** Get a single employee by id */
  async get(id: number): Promise<EmployeeDto> {
    const res = await fetch(`/api/employees/${id}`, { credentials: "include" });
    return handle<EmployeeDto>(res);
  },

  /** Create a new employee */
  async create(payload: EmployeeCreateDto): Promise<EmployeeDto> {
    const res = await fetch("/api/employees", {
      method: "POST",
      headers: JSON_HEADERS,
      credentials: "include",
      body: JSON.stringify(payload),
    });
    return handle<EmployeeDto>(res);
  },

  /** Update an existing employee (full overwrite of mutable fields) */
  async update(id: number, payload: EmployeeUpdateDto): Promise<void> {
    const res = await fetch(`/api/employees/${id}`, {
      method: "PUT",
      headers: JSON_HEADERS,
      credentials: "include",
      body: JSON.stringify(payload),
    });
    return handle<void>(res);
  },

  /** Delete an employee by id */
  async remove(id: number): Promise<void> {
    const res = await fetch(`/api/employees/${id}`, {
      method: "DELETE",
      credentials: "include",
    });
    return handle<void>(res);
  },

  /**
   * Runs the special increment rule on the server and returns the sum for A/B/C names
   * when the threshold (>=11171) is met; otherwise the API returns 204 No Content.
   */
  async runRule(): Promise<AbcSumResponse | null> {
    const res = await fetch(`/api/employees/update-values-and-sum`, {
      method: "POST",
      credentials: "include",
    });
    if (res.status === 204) return null;
    return handle<AbcSumResponse>(res);
  },
};

export default EmployeesApi;
