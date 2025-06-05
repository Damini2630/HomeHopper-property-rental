import React from 'react';

function EditLeaseAgreementForm({ editFormData, handleEditFormChange, handleUpdateLease, onCancel }) {
  return (
    <form onSubmit={handleUpdateLease} className="edit-form">
      <div className="form-group">
        <label htmlFor="propertyID">Property ID</label>
        <input type="text" id="propertyID" name="propertyID" value={editFormData.propertyID} onChange={handleEditFormChange} readOnly />
      </div>
      <div className="form-group">
        <label htmlFor="startDate">Start Date</label>
        <input type="date" id="startDate" name="startDate" value={editFormData.startDate} onChange={handleEditFormChange} />
      </div>
      <div className="form-group">
        <label htmlFor="endDate">End Date</label>
        <input type="date" id="endDate" name="endDate" value={editFormData.endDate} onChange={handleEditFormChange} />
      </div>
      <div className="form-group">
        <label htmlFor="tenantSignaturePath">Tenant Signature Path</label>
        <input type="text" id="tenantSignaturePath" name="tenantSignaturePath" value={editFormData.tenantSignaturePath} onChange={handleEditFormChange} />
      </div>
      <div className="form-group">
        <label htmlFor="tenantDocumentPath">Tenant Document Path</label>
        <input type="text" id="tenantDocumentPath" name="tenantDocumentPath" value={editFormData.tenantDocumentPath} onChange={handleEditFormChange} />
      </div>
      <div className="form-actions">
        <button type="submit" className="btn-primary">Save</button>
        <button type="button" className="btn-secondary" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  );
}

export default EditLeaseAgreementForm;