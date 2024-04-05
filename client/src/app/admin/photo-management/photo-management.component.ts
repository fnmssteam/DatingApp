import { Component } from '@angular/core';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent {
  unapprovedPhotos?: Photo[]
  pagination?: Pagination;
  pageNumber = 1;
  pageSize = 5;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.getPhotos()
  }

  getPhotos() {
    this.adminService.getUnapprovedPhotos(this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.unapprovedPhotos = response.result
        this.pagination = response.pagination;
      }
    })
  }

  approvePhoto(id: number) {
    this.adminService.approvePhoto(id).subscribe({
      next: () => {
        this.unapprovedPhotos = this.unapprovedPhotos?.filter(p => p.id !== id)
      }
    })
  }

  rejectPhoto(id: number) {
    this.adminService.rejectPhoto(id).subscribe({
      next: () => {
        this.unapprovedPhotos = this.unapprovedPhotos?.filter(p => p.id !== id)
      }
    })
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.getPhotos();
    }
  }
}
