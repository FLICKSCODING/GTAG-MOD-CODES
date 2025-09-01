public static void TagGunW()
{
    bool rightClick = Input.GetMouseButton(1); // Right mouse button
    if (rightClick)
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out raycastHit);

        bool flag = pointer == null;
        if (flag)
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(pointer.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(pointer.GetComponent<SphereCollider>());
            pointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            pointer.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            pointer.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
        pointer.transform.position = raycastHit.point;

        bool flag2 = Input.GetMouseButtonDown(0) && !PointerBreak; // Left mouse button activates
        if (flag2)
        {
            pointer.gameObject.GetComponent<Renderer>().material.color = Color.blue;
            bool flag3 = raycastHit.collider.GetComponentInParent<VRRig>();
            if (flag3)
            {
                bool flag4 = !PhotonNetwork.IsMasterClient;
                if (flag4)
                {
                    Player.Instance.rightControllerTransform.position = raycastHit.collider.GetComponentInParent<VRRig>().transform.position;
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = raycastHit.collider.GetComponentInParent<VRRig>().transform.position;
                }
                else
                {
                    GorillaTagManager component = GameObject.Find("Gorilla Tag Manager").GetComponent<GorillaTagManager>();
                    component.AddInfectedPlayer(raycastHit.collider.GetComponentInParent<VRRig>().Creator);
                }
                PointerBreak = true;
            }
        }
        else
        {
            pointer.gameObject.GetComponent<Renderer>().material.color = Color.white;
            PointerBreak = false;
            GorillaTagger.Instance.offlineVRRig.enabled = true;
        }
    }
    else
    {
        // Destroy pointer when not holding right-click
        if (pointer != null)
        {
            UnityEngine.Object.Destroy(pointer);
            pointer = null;
        }
    }
}
