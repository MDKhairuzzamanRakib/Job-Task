import React, { useEffect, useState } from "react";
import api from "../api";

const UserTable = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  // Pagination states
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [totalRecords, setTotalRecords] = useState(0);

  const fetchUsers = async (pageNumber = 1) => {
    setLoading(true);
    try {
      // 👇 Call the API with pageNumber and pageSize as query params
      const res = await api.get(`/fetch-users?pageNumber=${pageNumber}&pageSize=${pageSize}`);

      // ✅ Update state based on the response
      setUsers(res.data.data);
      setTotalPages(res.data.totalPages);
      setTotalRecords(res.data.totalRecords);
      setCurrentPage(res.data.currentPage);
    } catch (err) {
      console.error("Error fetching users:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers(currentPage);
  }, []);

  const handleNextPage = () => {
    if (currentPage < totalPages) {
      const nextPage = currentPage + 1;
      fetchUsers(nextPage);
    }
  };

  const handlePrevPage = () => {
    if (currentPage > 1) {
      const prevPage = currentPage - 1;
      fetchUsers(prevPage);
    }
  };

  if (loading) return <p>Loading users...</p>;

  return (
    <div className="table-container">
      <h2>User List</h2>

      <table border="1" cellPadding="8" style={{ width: "100%", textAlign: "left" }}>
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Age</th>
            <th>Email</th>
            <th>TimeStamp</th>
          </tr>
        </thead>
        <tbody>
          {users.length > 0 ? (
            users.map((u) => (
              <tr key={u.id}>
                <td>{u.id}</td>
                <td>{u.name}</td>
                <td>{u.age}</td>
                <td>{u.email}</td>
                <td>{new Date(u.timeStamp).toLocaleString()}</td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="5" align="center">
                No users found
              </td>
            </tr>
          )}
        </tbody>
      </table>

      {/* Pagination Controls */}
      {totalPages > 1 && (
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            marginTop: "15px",
            gap: "10px",
          }}
        >
          <button
            onClick={handlePrevPage}
            disabled={currentPage === 1}
            style={{ padding: "6px 10px" }}
          >
            ◀ Prev
          </button>

          <span>
            Page <b>{currentPage}</b> of <b>{totalPages}</b> ({totalRecords} users)
          </span>

          <button
            onClick={handleNextPage}
            disabled={currentPage === totalPages}
            style={{ padding: "6px 10px" }}
          >
            Next ▶
          </button>
        </div>
      )}
    </div>
  );
};

export default UserTable;
