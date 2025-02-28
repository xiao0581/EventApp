import { useAuthStore } from 'src/stores/auth'

export const getSasToken = async (): Promise<string> => {
  //to do : parameterize the permission
  try {
    const authStore = useAuthStore()
    const token = authStore.user?.token

    if (!token) {
      throw new Error('User not authenticated')
    }

    const requestBody = {
      blobName: '',
      permission: 'w',
      expiryMinutes: 60,
    }

    const response = await fetch('http://localhost:5102/api/generate-sas-token', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody),
    })

    if (!response.ok) {
      throw new Error('Failed to get SAS Token')
    }

    const data = await response.json()
    return data.sasToken
  } catch (error) {
    console.error('Error fetching SAS Token:', error)
    throw new Error('Failed to get SAS Token')
  }
}

export const uploadToAzureBlob = async (file: File): Promise<string> => {
  try {
    const containerName = 'eversayde'
    const accountName = 'eversaydevne'

    const sasToken = await getSasToken()

    const blobUrl = `https://${accountName}.blob.core.windows.net/${containerName}/${file.name}?${sasToken}`

    const response = await fetch(blobUrl, {
      method: 'PUT',
      headers: {
        'x-ms-blob-type': 'BlockBlob',
        'Content-Type': file.type,
      },
      body: file,
    })

    if (!response.ok) {
      throw new Error('Upload to Azure failed')
    }

    const urlWithoutSasToken = blobUrl.split('?')[0]
    if (!urlWithoutSasToken) {
      throw new Error('Failed to parse blob URL')
    }
    return urlWithoutSasToken
  } catch (error) {
    console.error('Azure upload error:', error)
    throw new Error('Failed to upload to Azure')
  }
}
