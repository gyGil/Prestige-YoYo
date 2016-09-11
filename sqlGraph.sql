SELECT DISTINCT COUNT(sinNumber) FROM manufacturingData
	WHERE resultState != '';

SELECT DISTINCT COUNT(sinNumber) FROM manufacturingData
	WHERE resultState = 'INCONSISTENT_THICKNESS' OR
	resultState = 'PITTING' OR
	resultState = 'WARPING' OR
	resultState = 'PRIMER_DEFECT' OR
	resultState = 'BROKEN_SHELL';

SELECT DISTINCT COUNT(sinNumber) FROM manufacturingData;