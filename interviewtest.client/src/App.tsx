import { useEffect, useState } from 'react';

function App() {
    const [employeeCount, setEmployeeCount] = useState<number>(0);

    useEffect(() => {
        checkConnectivity();
    }, []);

    return (<>
        <div>Connectivity check: {employeeCount > 0 ? `OK (${employeeCount})` : `NOT READY`}</div>
        <div>Complete your app here</div>
    </>);

    async function checkConnectivity() {
        const response = await fetch('api/employees');
        const data = await response.json();
        setEmployeeCount(data.length);
    }
}

export default App;